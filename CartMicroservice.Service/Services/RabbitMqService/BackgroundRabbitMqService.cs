using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CartMicroservice.Repository.Repository;
using CartMicroservice.Service.Services.RabbitMqService.Models;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace CartMicroservice.Service.Services.RabbitMqService
{
    public class BackgroundRabbitMqService : BackgroundService
    {
        private const string RequestQueueName = "requestcart";
        private readonly IModel _channel;
        private readonly IRepositoryGeneric<Domain.Entities.Cart> _repository;
        private readonly IRabbitMqService _rabbitMqService;
        private readonly IMapper _mapper;

        public BackgroundRabbitMqService(IRepositoryGeneric<Domain.Entities.Cart> repository,
            IRabbitMqService rabbitMqService, IMapper mapper)
        {
            _mapper = mapper;
            _rabbitMqService = rabbitMqService;
            _repository = repository;

            var factory = new ConnectionFactory();
            var connection = factory.CreateConnection();
            _channel = connection.CreateModel();

            _channel.QueueDeclare(RequestQueueName, true, false, false, null);
        }

        private async void Consumer_Received(object sender, BasicDeliverEventArgs e)
        {
            var requestMessage = Encoding.UTF8.GetString(e.Body.ToArray());
            var cartId = int.Parse(requestMessage);
            var correlationId = e.BasicProperties.CorrelationId;
            var responseQueueName = e.BasicProperties.ReplyTo;

            var responseMessage = await GetProductsInfo(cartId);

            Publish(responseMessage, correlationId, responseQueueName);
        }

        private async Task<string> GetProductsInfo(int cartId)
        {
            var cart = await _repository.GetByIdAsync(cartId);
            var products = _mapper.Map<IEnumerable<ProductsRequestModel>>(cart.Products.Where(_ => _.DeletedDate == null));
            var response = await _rabbitMqService.SendAsync(products);
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