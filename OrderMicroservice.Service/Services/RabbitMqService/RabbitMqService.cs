using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Concurrent;
using System.Text;
using System.Threading.Tasks;

namespace OrderMicroservice.Service.Services.RabbitMqService
{
    public class RabbitMqService : IRabbitMqService, IDisposable
    {
        private bool _disposed;
        private readonly IConnection _connection;
        private readonly IModel _channel;

        private static ConcurrentDictionary<string,
            TaskCompletionSource<string>> _pendingMessages;

        private const string RequestCartQueueName = "requestcart";
        private const string ResponseCartQueueName = "responsecart";
        private const string RequestAddressQueueName = "requestaddress";
        private const string ResponseAddressQueueName = "responseaddress";
        private const string RequestTotalPriceQueueName = "requesttotalprice";
        private const string ResponseTotalPriceQueueName = "responsetotalprice";
        private const string RequestLockTheCartQueueName = "requestlockthecart";
        private const string ExchangeName = "";


        public RabbitMqService()
        {
            var factory = new ConnectionFactory();

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.QueueDeclare(RequestCartQueueName, true, false, false, null);
            _channel.QueueDeclare(ResponseCartQueueName, true, false, false, null);
            _channel.QueueDeclare(RequestAddressQueueName, true, false, false, null);
            _channel.QueueDeclare(ResponseAddressQueueName, true, false, false, null);
            _channel.QueueDeclare(RequestTotalPriceQueueName, true, false, false, null);
            _channel.QueueDeclare(ResponseTotalPriceQueueName, true, false, false, null);

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += Consumer_Received;
            _channel.BasicConsume(ResponseCartQueueName, true, consumer);
            _channel.BasicConsume(ResponseAddressQueueName, true, consumer);
            _channel.BasicConsume(ResponseTotalPriceQueueName, true, consumer);

            _pendingMessages = new ConcurrentDictionary<string,
                TaskCompletionSource<string>>();
        }

        private void Consumer_Received(object sender, BasicDeliverEventArgs e)
        {
            var correlationId = e.BasicProperties.CorrelationId;
            var json = Encoding.UTF8.GetString(e.Body.ToArray());
            _pendingMessages.TryRemove(correlationId, out var tcs);
            tcs?.SetResult(json);
        }

        public Task<string> GetAddressInfo(int addressId)
        {
            var tcs = new TaskCompletionSource<string>();

            var correlationId = Guid.NewGuid().ToString();

            _pendingMessages[correlationId] = tcs;

            Publish(addressId, correlationId, ResponseAddressQueueName, RequestAddressQueueName);

            return tcs.Task;
        }

        public Task<string> GetProductsInfo(int cartId)
        {
            var tcs = new TaskCompletionSource<string>();

            var correlationId = Guid.NewGuid().ToString();

            _pendingMessages[correlationId] = tcs;

            Publish(cartId, correlationId, ResponseCartQueueName, RequestCartQueueName);

            return tcs.Task;
        }

        public Task<string> GetTotalPrice(int cartId)
        {
            var tcs = new TaskCompletionSource<string>();

            var correlationId = Guid.NewGuid().ToString();

            _pendingMessages[correlationId] = tcs;

            Publish(cartId, correlationId, ResponseTotalPriceQueueName, RequestTotalPriceQueueName);

            return tcs.Task;
        }

        public void LockTheCart(int cartId)
        {
            Publish(cartId, RequestLockTheCartQueueName);
        }

        private void Publish(int id, string correlationId, string responseQueueName, string requestQueueName)
        {
            var props = _channel.CreateBasicProperties();
            props.CorrelationId = correlationId;
            props.ReplyTo = responseQueueName;
            var strId = id.ToString();
            var body = Encoding.UTF8.GetBytes(strId);

            _channel.BasicPublish(ExchangeName, requestQueueName, props, body);

        }

        private void Publish (int id, string requestQueueName)
        {
            var props = _channel.CreateBasicProperties();
            var strId = id.ToString();
            var body = Encoding.UTF8.GetBytes(strId);

            _channel.BasicPublish(ExchangeName, requestQueueName, props, body);
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