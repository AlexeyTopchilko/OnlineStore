using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using OrderMicroservice.Domain.Entities;
using OrderMicroservice.Repository.Repository;
using OrderMicroservice.Service.Services.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using OrderMicroservice.Service.Services.OrderService;

namespace OrderMicroservice.Service.Services.RabbitMqService
{
    public class RabbitBackgroundService : BackgroundService
    {
        private const string RequestOrderInfo = "requestorderinfo";
        private const string RequestPaymentResult = "requestpaymentresult";

        private readonly IRepositoryGeneric<Order> _orderRepository;
        private readonly IOrderService _orderService;

        private readonly IModel _channel;

        public RabbitBackgroundService(IRepositoryGeneric<Order> orderRepository, IOrderService orderService)
        {
            _orderService = orderService;
            _orderRepository = orderRepository;
            var factory = new ConnectionFactory();
            var connection = factory.CreateConnection();
            _channel = connection.CreateModel();

            _channel.QueueDeclare(RequestOrderInfo, true, false, false, null);
            _channel.QueueDeclare(RequestPaymentResult, true, false, false, null);
        }

        private async Task<string> GetOrderInfo(int orderId)
        {
            var price = (await _orderRepository.GetByIdAsync(orderId)).TotalPrice;
            var orderinfo = new OrderInfo
            {
                OrderId = orderId,
                TotalPrice = price
            };

            var json = JsonConvert.SerializeObject(orderinfo);

            return json;
        }

        private async void Consumer_Received(object sender, BasicDeliverEventArgs e)
        {
            var requestMessage = Encoding.UTF8.GetString(e.Body.ToArray());
            switch (e.RoutingKey)
            {
                case RequestOrderInfo:
                    var orderId = int.Parse(requestMessage);
                    var correlationId = e.BasicProperties.CorrelationId;
                    var responseQueueName = e.BasicProperties.ReplyTo;
                    var responseMessage = await GetOrderInfo(orderId);
                    Publish(responseMessage, correlationId, responseQueueName);
                    break;
                case RequestPaymentResult:
                    var paymentInfo = JsonConvert.DeserializeObject<PaymentResult>(requestMessage);
                    await _orderService.TakePayment(paymentInfo);
                    break;
            }
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
            _channel.BasicConsume(RequestOrderInfo, true, consumer);
            _channel.BasicConsume(RequestPaymentResult, true, consumer);

            return Task.CompletedTask;
        }
    }
}