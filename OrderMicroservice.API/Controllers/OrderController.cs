using Microsoft.AspNetCore.Mvc;
using OrderMicroservice.Service.Services.OrderService;
using System.Threading.Tasks;

namespace OrderMicroservice.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet]
        [Route("GetOrderById")]
        public async Task<IActionResult> GetById(int orderId)
        {
            var order = await _orderService.GetByIdAsync(orderId);
            return Ok(order);
        }
    }
}