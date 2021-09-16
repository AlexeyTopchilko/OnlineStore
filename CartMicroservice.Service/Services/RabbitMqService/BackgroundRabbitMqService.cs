using AutoMapper;
using CartMicroservice.Domain.Entities;
using CartMicroservice.Repository.Repository;
using CartMicroservice.Service.Services.CartService;
using CartMicroservice.Service.Services.RabbitMqService.Models;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CartMicroservice.Service.Services.RabbitMqService
{
    public class BackgroundRabbitMqService : BackgroundService
    {
        private const string RequestProductsQueueName = "requestcart";
        private const string RequestTotalPriceQueueName = "requesttotalprice";
        private const string RequestLockTheCartQueueName = "requestlockthecart";
        private readonly IModel _channel;
        private readonly IRepositoryGeneric<Cart> _repository;
        private readonly IRabbitMqService _rabbitMqService;
        private readonly IMapper _mapper;
        private readonly ICartService _cartService;

        public BackgroundRabbitMqService(IRepositoryGeneric<Cart> repository,
            IRabbitMqService rabbitMqService, IMapper mapper, ICartService cartService)
        {
            _cartService = cartService;
            _mapper = mapper;
            _rabbitMqService = rabbitMqService;
            _repository = repository;

            var factory = new ConnectionFactory();
            var connection = factory.CreateConnection();
            _channel = connection.CreateModel();

            _channel.QueueDeclare(RequestProductsQueueName, true, false, false, null);
            _channel.QueueDeclare(RequestTotalPriceQueueName, true, false, false, null);
            _channel.QueueDeclare(RequestLockTheCartQueueName, true, false, false, null);
        }

        private async void Consumer_Received(object sender, BasicDeliverEventArgs e)
        {
            var requestMessage = Encoding.UTF8.GetString(e.Body.ToArray());
            var cartId = int.Parse(requestMessage);
            var correlationId = e.BasicProperties.CorrelationId;
            var responseQueueName = e.BasicProperties.ReplyTo;

            string responseMessage;
            switch (e.RoutingKey)
            {
                case RequestProductsQueueName:
                    responseMessage = await GetProductsInfo(cartId);
                    Publish(responseMessage, correlationId, responseQueueName);
                    break;
                case RequestTotalPriceQueueName:
                    responseMessage = await GetTotalPrice(cartId);
                    Publish(responseMessage, correlationId, responseQueueName);
                    break;
                case RequestLockTheCartQueueName:
                    await _cartService.LockTheCart(cartId);
                    break;
                default:
                    break;
            }
        }

        private async Task<string> GetProductsInfo(int cartId)
        {
            var cart = await _repository.GetByIdAsync(cartId);
            var products = _mapper.Map<IEnumerable<ProductsRequestModel>>(cart.Products.Where(_ => _.DeletedDate == null));
            var response = await _rabbitMqService.SendAsync(products);
            return response;
        }

        private async Task<string> GetTotalPrice(int cartId)
        {
            var totalPrice = (await _repository.GetByIdAsync(cartId)).TotalPrice.ToString();
            return totalPrice;
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
            _channel.BasicConsume(RequestProductsQueueName, true, consumer);
            _channel.BasicConsume(RequestTotalPriceQueueName, true, consumer);
            _channel.BasicConsume(RequestLockTheCartQueueName, true, consumer);

            return Task.CompletedTask;
        }
    }
}