using System.Threading.Tasks;
using PaymentMicroservice.Service.Services.Models;

namespace PaymentMicroservice.Service.Services.RabbitMqService
{
    public interface IRabbitMqService
    {
        Task<string> GetOrderInfo(int orderId);

        void SendPaymentResult(PaymentResult result);

    }
}