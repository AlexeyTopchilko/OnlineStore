using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CatalogMicroservice.Domain.Entities;
using CatalogMicroservice.Repository.Repository;
using CatalogMicroservice.Service.Services.RabbitMqService.Models;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace CatalogMicroservice.Service.Services.RabbitMqService
{
    public class RabbitMqService : BackgroundService
    {
        private const string RequestQueueName = "requestproductsinfo";

        private readonly IRepositoryGeneric<Product> _productRepository;

        private readonly IModel _channel;

        public RabbitMqService(IRepositoryGeneric<Product> productRepository)
        {
            _productRepository = productRepository;
            var factory = new ConnectionFactory();
            var connection = factory.CreateConnection();
            _channel = connection.CreateModel();

            _channel.QueueDeclare(RequestQueueName, true, false, false, null);
        }
        private async void Consumer_Received(object sender, BasicDeliverEventArgs e)
        {
            var requestMessage = Encoding.UTF8.GetString(e.Body.ToArray());
            var model = JsonConvert.DeserializeObject<IEnumerable<ProductsRequestModel>>(requestMessage);
            var correlationId = e.BasicProperties.CorrelationId;
            var responseQueueName = e.BasicProperties.ReplyTo;

            var responseMessage = await GetData(model);
            Publish(responseMessage, correlationId, responseQueueName);
        }

        private async Task<string> GetData(IEnumerable<ProductsRequestModel> model)
        {
            var ids = model.Select(item => item.ProductId).ToList();

            var products = await _productRepository.GetByPredicateAsync(_ => ids.Contains(_.Id));

            var responseList = (from item in products
                                let requestModel = model.FirstOrDefault(_ => _.ProductId == item.Id)
                                where requestModel != null
                                select new ProductsResponseModel()
                                {
                                    Id = requestModel.Id,
                                    ProductId = item.Id,
                                    Name = item.Name,
                                    Price = item.Price,
                                    Quantity = requestModel.Quantity,
                                    TotalPrice = item.Price * requestModel.Quantity,
                                    Image = item.Image
                                }).ToList();

            var response = JsonConvert.SerializeObject(responseList);
            return response;

        }

        private void Publish(string responseMessage, string correlationId,
            string responseQueueName)
        {
            var responseMessageBytes = Encoding.UTF8.GetBytes(responseMessage);

            const string exchangeName = "";
            var responseProps = _channel.CreateBasicProperties();
            responseProps.CorrelationId = correlationId;

            _channel.BasicPublish(exchangeName, responseQueueName, responseProps, responseMessageBytes);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += Consumer_Received;
            _channel.BasicConsume(RequestQueueName, true, consumer);

            return Task.CompletedTask;
        }
    }
}