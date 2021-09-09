using AddressMicroservice.Repository.Repository;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AddressMicroservice.Service.Services.RabbitMqService
{
    public class BackgroundRabbitMqService : BackgroundService
    {
        private const string RequestQueueName = "requestaddress";

        private readonly IRepositoryGeneric<Domain.Entities.Address> _addressRepository;

        private readonly IModel _channel;

        public BackgroundRabbitMqService(IRepositoryGeneric<Domain.Entities.Address> addressRepository)
        {
            _addressRepository = addressRepository;

            var factory = new ConnectionFactory();
            var connection = factory.CreateConnection();
            _channel = connection.CreateModel();

            _channel.QueueDeclare(RequestQueueName, true, false, false, null);
        }

        private async void Consumer_Received(object sender, BasicDeliverEventArgs e)
        {
            var requestMessage = Encoding.UTF8.GetString(e.Body.ToArray());
            var id = int.Parse(requestMessage);
            var correlationId = e.BasicProperties.CorrelationId;
            var responseQueueName = e.BasicProperties.ReplyTo;

            var responseMessage = await GetAddress(id);
            Publish(responseMessage, correlationId, responseQueueName);
        }

        private async Task<string> GetAddress(int id)
        {
            var address = await _addressRepository.GetByIdAsync(id);

            var responseModel = new
            {
                Id = address.Id,
                City = address.City,
                Street = address.Street,
                HouseNumber = address.HouseNumber
            };

            var response = JsonConvert.SerializeObject(responseModel);

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