using System.Threading.Tasks;
using Newtonsoft.Json;
using PaymentMicroservice.Service.Services.Models;
using PaymentMicroservice.Service.Services.RabbitMqService;
using Stripe;

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

        public async Task<Response> PayAsync(string cardNumber, int month, int year, string cvc, int orderId)
        {
            var json = await _rabbitMqService.GetOrderInfo(orderId);
            var orderinfo = JsonConvert.DeserializeObject<OrderInfo>(json);

            var optionstoken = new TokenCreateOptions
            {
                Card = new TokenCardOptions
                {
                    Number = cardNumber,
                    ExpMonth = month,
                    ExpYear = year,
                    Cvc = cvc
                }
            };

            var serviceToken = new TokenService();
            var stripeToken = await serviceToken.CreateAsync(optionstoken);

            var options = new ChargeCreateOptions
            {
                Amount = (int)(orderinfo.TotalPrice * 100),
                Currency = "usd",
                Description = "OnlineShop",
                Source = stripeToken.Id
            };

            var service = new ChargeService();
            var charge = await service.CreateAsync(options);

            if (!charge.Paid) return new Response {Status = "Failed!", Message = "Something went wrong!"};

            _rabbitMqService.SendPaymentResult(new PaymentResult
            {
                Paid = true,
                OrderId = orderId
            });

            return new Response {Status = "Success", Message = "Paid successfully!"};
        }
    }
}