using System.Threading.Tasks;

namespace OrderMicroservice.Service.Services.RabbitMqService
{
    public interface IRabbitMqService
    {
        Task<string> GetProductsInfo(int cartId);

        Task<string> GetAddressInfo(int addressId);

        Task<string> GetTotalPrice(int cartId);

        void LockTheCart(int cartId);
    }
}