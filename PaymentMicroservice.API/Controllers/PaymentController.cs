using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PaymentMicroservice.Service.Services;
using PaymentMicroservice.Service.Services.Models;
using Stripe;

namespace PaymentMicroservice.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpGet]
        [Route("GetOrderInfo")]
        public async Task<IActionResult> GetInfo(int orderId)
        {
            var info = await _paymentService.GetOrderInfo(orderId);
            return Ok(info);
        }

        [HttpPost]
        [Route("Pay")]
        public void Pay([FromBody]PaymentResult result)
        {
            _paymentService.Pay(result);
        }

        [HttpPost]
        [Route("PayStripe")]
        public async Task<IActionResult> PayWithStripe([FromBody] PaymentModel model)
        {
            try
            {
                var result = await _paymentService.PayAsync(model.CardNumber, model.Month, model.Year, model.Cvc,
                    model.OrderId);
                return Ok(result);
            }
            catch (StripeException ex)
            {
                return Ok(new Response{ Status = "Failed!", Message = ex.Message});
            }

        }
    }
}