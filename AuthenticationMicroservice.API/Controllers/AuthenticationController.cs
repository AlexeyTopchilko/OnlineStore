using AuthenticationMicroservice.Service.Models;
using AuthenticationMicroservice.Service.Services.UserService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;

namespace AuthenticationMicroservice.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IUserService _userService;


        public AuthenticationController(IUserService service)
        {
            _userService = service;
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            try
            {
                var identity = await _userService.GetIdentityAsync(model);
                if (identity == null)
                {
                    return StatusCode(StatusCodes.Status501NotImplemented, new Response { Status = "Error", Message = "Invalid username or password!" });
                }

                var jwt = _userService.CreateToken(identity);

                var encodedJwt = _userService.EncodeJwt(jwt);
                var user = await _userService.GetUserByName(model.Username);
                var response = new
                {
                    access_token = encodedJwt,
                    username = identity.Name,
                    id = user.Id
                };

                return Ok(response);
            }
            catch (SqlException e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { errorText = $"Something going wrong! \n {e.Message} " });
            }
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            try
            {
                if (await _userService.IsEmailExist(model.Email) || await _userService.IsUsernameExist(model.Username))
                    return StatusCode(StatusCodes.Status501NotImplemented, new Response { Status = "Error", Message = "Username or email already exists!" });

                await _userService.AddAsync(model);
                return Ok(new Response { Status = "Success", Message = "User created successfully!" });
            }
            catch (SqlException e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { errorText = $"Something going wrong! \n {e.Message} " });
            }
        }
    }
}