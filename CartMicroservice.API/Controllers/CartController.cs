using CartMicroservice.Service.Services.CartService;
using CartMicroservice.Service.Services.CartService.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System;
using System.Threading.Tasks;

namespace CartMicroservice.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        [HttpGet]
        [Route("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var response = await _cartService.GetAllAsync();
                return Ok(response);
            }
            catch (SqlException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { errorText = $"Something going wrong! \n {ex.Message} " });
            }
        }

        [HttpGet]
        [Route("GetActiveByUser")]
        public async Task<IActionResult> GetActiveByUser(Guid userId)
        {
            try
            {
                var order = await _cartService.GetActiveCartByUser(userId);

                return order != null ? Ok(order) : Ok(new CartViewModel());
            }
            catch (SqlException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { errorText = $"Something going wrong! \n {ex.Message} " });
            }
        }

        [HttpPost]
        [Route("AddToCart")]
        public async Task<IActionResult> AddToCart(AddToCartModel model)
        {
            try
            {
                await _cartService.AddToCart(model);

                var response = new
                { Message = "Added successfully!" };

                return Ok(response);
            }
            catch (SqlException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { errorText = $"Something going wrong! \n {ex.Message} " });
            }
        }

        [HttpPost]
        [Route("LockCart")]
        public async Task<IActionResult> LockCart(int orderId)
        {
            try
            {
                await _cartService.LockTheCart(orderId);
                return Ok();
            }
            catch (SqlException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { errorText = $"Something going wrong! \n {ex.Message} " });
            }
        }

        [HttpGet]
        [Route("GetTotalQuantity")]
        public async Task<IActionResult> GetTotalQuantity(Guid userId)
        {
            try
            {
                var totalQuantity = await _cartService.GetTotalQuantity(userId);
                return Ok(totalQuantity);
            }
            catch (SqlException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { errorText = $"Something going wrong! \n {ex.Message} " });
            }
        }

        [HttpPost]
        [Route("UpdateCart")]
        public async Task<IActionResult> UpdateCartAsync(CartViewModel model)
        {
            try
            {
                await _cartService.UpdateCart(model);
                return Ok("Updated successfully!");
            }
            catch (SqlException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { errorText = $"Something going wrong! \n {ex.Message} " });
            }
        }
    }
}