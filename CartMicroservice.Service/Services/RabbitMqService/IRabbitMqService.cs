using CartMicroservice.Service.Services.RabbitMqService.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CartMicroservice.Service.Services.RabbitMqService
{
    public interface IRabbitMqService
    {
        Task<string> SendAsync(IEnumerable<ProductsRequestModel> products);
    }
}