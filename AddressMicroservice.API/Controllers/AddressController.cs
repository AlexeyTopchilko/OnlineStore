using AddressMicroservice.Service.Services.AddressService;
using AddressMicroservice.Service.Services.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AddressMicroservice.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AddressController : ControllerBase
    {
        private readonly IAddressService _addressService;
        public AddressController(IAddressService addressService)
        {
            _addressService = addressService;
        }

        [HttpGet]
        [Route("GetUserAddresses")]
        public async Task<IActionResult> GetUserAddresses(Guid userId)
        {
            try
            {
                var addresses = await _addressService.GetUserAddresses(userId);
                return addresses.Any() ? Ok(addresses) : StatusCode(StatusCodes.Status404NotFound, new { errorText = "No addresses for user" });
            }
            catch (SqlException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { errorText = $"Something going wrong! \n {ex.Message} " });
            }
        }

        [HttpPost]
        [Route("CreateAddress")]
        public async Task<IActionResult> AddAddress(AddAddressModel model)
        {
            try
            {
                await _addressService.AddAddress(model);
                return Ok(new { message = "created successfully" });
            }
            catch (SqlException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { errorText = $"Something going wrong! \n {ex.Message} " });
            }
        }

        [HttpDelete]
        [Route("DeleteAddress")]
        public async Task<IActionResult> DeleteAddress(int addressId)
        {
            try
            {
                await _addressService.DeleteAddress(addressId);
                return Ok(new { message = "Deleted successfully" });
            }
            catch (SqlException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { errorText = $"Something going wrong! \n {ex.Message} " });
            }
        }
    }
}