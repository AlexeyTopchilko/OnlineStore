using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using CartMicroservice.Repository.Repository;
using CartMicroservice.Service.Services.RabbitMqService.Models;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace CartMicroservice.Service.Services.RabbitMqService
{
    public class RabbitMqService : IRabbitMqService, IDisposable
    {
        private bool _disposed;
        private readonly IConnection _connection;
        private readonly IModel _channel;

        private static ConcurrentDictionary<string,
            TaskCompletionSource<string>> _pendingMessages;

        private const string RequestProductsQueueName = "requestproductsinfo";
        private const string ResponseProductsQueueName = "responseproductsinfo";


        private const string ExchangeName = "";

        private readonly IRepositoryGeneric<Domain.Entities.Cart> _cartRepository;
        private readonly IMapper _mapper;

        public RabbitMqService(IRepositoryGeneric<Domain.Entities.Cart> cartRepository, IMapper mapper)
        {
            _mapper = mapper;
            _cartRepository = cartRepository;
            var factory = new ConnectionFactory();

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.QueueDeclare(RequestProductsQueueName, true, false, false, null);
            _channel.QueueDeclare(ResponseProductsQueueName, true, false, false, null);

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += Consumer_Received;
            _channel.BasicConsume(ResponseProductsQueueName, true, consumer);

            _pendingMessages = new ConcurrentDictionary<string,
                TaskCompletionSource<string>>();
        }

        public Task<string> SendAsync(IEnumerable<ProductsRequestModel> products)
        {
            var tcs = new TaskCompletionSource<string>();

            var correlationId = Guid.NewGuid().ToString();

            _pendingMessages[correlationId] = tcs;

            Publish(products, correlationId);

            return tcs.Task;
        }

        private void Publish(IEnumerable<ProductsRequestModel> productIds, string correlationId)
        {
            var props = _channel.CreateBasicProperties();
            props.CorrelationId = correlationId;
            props.ReplyTo = ResponseProductsQueueName;
            var json = JsonConvert.SerializeObject(productIds);
            var body = Encoding.UTF8.GetBytes(json);

            _channel.BasicPublish(ExchangeName, RequestProductsQueueName, props, body);

        }

        private void Consumer_Received(object sender, BasicDeliverEventArgs e)
        {

            var correlationId = e.BasicProperties.CorrelationId;
            var json = Encoding.UTF8.GetString(e.Body.ToArray());
            _pendingMessages.TryRemove(correlationId, out var tcs);
            tcs?.SetResult(json);

        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                _channel?.Dispose();
                _connection?.Dispose();
            }

            _disposed = true;
        }
    }
}