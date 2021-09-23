using System.Threading.Tasks;
using Newtonsoft.Json;
using PaymentMicroservice.Service.Services.Models;
using PaymentMicroservice.Service.Services.RabbitMqService;

namespace PaymentMicroservice.Service.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IRabbitMqService _rabbitMqService;

        public PaymentService(IRabbitMqService rabbitMqService)
        {
            _rabbitMqService = rabbitMqService;
        }

        public async Task<OrderInfo> GetOrderInfo(int orderId)
        {
            var json = await _rabbitMqService.GetOrderInfo(orderId);
            var orderinfo = JsonConvert.DeserializeObject<OrderInfo>(json);
            return orderinfo;
        }

        public void Pay(PaymentResult result)
        {
            _rabbitMqService.SendPaymentResult(result);
        }
    }
}