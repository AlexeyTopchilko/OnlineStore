using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using OrderMicroservice.Service.Services.Models;
using OrderMicroservice.Service.Services.OrderService;
using System;
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
            try
            {
                var order = await _orderService.GetByIdAsync(orderId);
                return Ok(order);
            }
            catch (SqlException e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { errorText = $"Something going wrong! \n {e.Message} " });
            }
        }

        [HttpGet]
        [Route("GetOrdersByUserId")]
        public async Task<IActionResult> GetByUserId(Guid userId)
        {
            try
            {
                var orders = await _orderService.GetByUserId(userId);
                return Ok(orders);
            }
            catch (SqlException e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { errorText = $"Something going wrong! \n {e.Message} " });
            }
        }

        [HttpPost]
        [Route("FormAnOrder")]
        public async Task<IActionResult> CreateOrder([FromBody] FormAnOrderModel model)
        {
            try
            {
                var orderId = await _orderService.FormAnOrder(model);
                return Ok(new { orderId = orderId.ToString() });
            }
            catch (SqlException e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { errorText = $"Something going wrong! \n {e.Message} " });
            }
        }

        [HttpPut]
        [Route("ConfirmOrder")]
        public async Task<IActionResult> ConfirmOrder(int orderId)
        {
            try
            {
                await _orderService.ConfirmOrder(orderId);
                return Ok();
            }
            catch (SqlException e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { errorText = $"Something going wrong! \n {e.Message} " });
            }
        }
    }
}