using System;
using System.Collections.Concurrent;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PaymentMicroservice.Service.Services.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace PaymentMicroservice.Service.Services.RabbitMqService
{
    public class RabbitMqService : IRabbitMqService, IDisposable
    {
        private bool _disposed;
        private readonly IConnection _connection;
        private readonly IModel _channel;

        private const string ExchangeName = "";

        private const string RequestOrderInfo = "requestorderinfo";
        private const string ResponseOrderInfo = "responseorderinfo";
        private const string RequestPaymentResult = "requestpaymentresult";

        private static ConcurrentDictionary<string,
            TaskCompletionSource<string>> _pendingMessages;

        public RabbitMqService()
        {
            var factory = new ConnectionFactory { HostName = "rabbitmq", UserName = "guest", Password = "guest", };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.QueueDeclare(RequestOrderInfo, true, false, false, null);
            _channel.QueueDeclare(ResponseOrderInfo, true, false, false, null);
            _channel.QueueDeclare(RequestPaymentResult, true, false, false, null);


            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += Consumer_Received;
            _channel.BasicConsume(ResponseOrderInfo, true, consumer);

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

        public Task<string> GetOrderInfo(int orderId)
        {
            var tcs = new TaskCompletionSource<string>();

            var correlationId = Guid.NewGuid().ToString();

            _pendingMessages[correlationId] = tcs;

            Publish(orderId, correlationId, ResponseOrderInfo, RequestOrderInfo);

            return tcs.Task;
        }

        public void SendPaymentResult(PaymentResult result)
        {
            var json = JsonConvert.SerializeObject(result);
            Publish(json, RequestPaymentResult);
        }

        private void Publish(int orderId, string correlationId, string responseQueueName, string requestQueueName)
        {
            var props = _channel.CreateBasicProperties();
            props.CorrelationId = correlationId;
            props.ReplyTo = responseQueueName;
            var strId = orderId.ToString();
            var body = Encoding.UTF8.GetBytes(strId);

            _channel.BasicPublish(ExchangeName, requestQueueName, props, body);
        }

        private void Publish(string message, string requestQueueName)
        {
            var props = _channel.CreateBasicProperties();

            var body = Encoding.UTF8.GetBytes(message);

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