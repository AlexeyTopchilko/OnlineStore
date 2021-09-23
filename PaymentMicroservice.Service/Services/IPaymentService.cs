using System.Threading.Tasks;
using PaymentMicroservice.Service.Services.Models;

namespace PaymentMicroservice.Service.Services
{
    public interface IPaymentService
    {
        void Pay(PaymentResult result);

        Task<OrderInfo> GetOrderInfo(int orderId);
    }
}