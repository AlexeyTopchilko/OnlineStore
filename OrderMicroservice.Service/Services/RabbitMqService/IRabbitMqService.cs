using System.Threading.Tasks;

namespace OrderMicroservice.Service.Services.RabbitMqService
{
    public interface IRabbitMqService
    {
        Task<string> GetCartInfo(int cartId);

        Task<string> GetAddressInfo(int addressId);
    }
}