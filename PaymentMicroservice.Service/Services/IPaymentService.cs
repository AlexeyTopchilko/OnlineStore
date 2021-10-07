using System.Threading.Tasks;
using PaymentMicroservice.Service.Services.Models;

namespace PaymentMicroservice.Service.Services
{
    public interface IPaymentService
    {
        void Pay(PaymentResult result);

        Task<OrderInfo> GetOrderInfo(int orderId);

        Task<Response> PayAsync(string cardNumber, int month, int year, string cvc, int orderId);
    }
}