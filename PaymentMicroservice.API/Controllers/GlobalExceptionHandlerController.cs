using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace PaymentMicroservice.API.Controllers
{
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class GlobalExceptionHandlerController : ControllerBase
    {
        [Route("/errors")]
        public IActionResult HandleErrors()
        {
            var contextException = HttpContext.Features.Get<IExceptionHandlerFeature>();
            var responseStatusCode = contextException.Error.GetType().Name switch
            {
                _ => HttpStatusCode.BadRequest
            };

            return Problem(detail: contextException.Error.Message, statusCode: (int)responseStatusCode);
        }
    }
}