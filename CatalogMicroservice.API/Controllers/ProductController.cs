using CatalogMicroservice.Service.Services.ProductService;
using CatalogMicroservice.Service.Services.ProductService.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace CatalogMicroservice.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService service)
        {
            _productService = service;
        }

        [HttpGet]
        [Route("Products")]
        public async Task<IActionResult> GetProducts(int sortMode, int skip, int take, int? categoryId = null, [MaxLength(15)]string name = null)
        {
            try
            {
                var productsView = await _productService.GetProductsAsync(sortMode, skip, take, categoryId, name);
                return productsView != null ? Ok(productsView) : Ok(new ProductsView());
            }
            catch (SqlException e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { errorText = $"Something going wrong! \n {e.Message} " });
            }
        }

        [HttpGet]
        [Route("ProductById")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var product = await _productService.GetProductByIdAsync(id);
                return product != null ? Ok(product) : Ok(new ProductView());
            }
            catch (SqlException e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { errorText = $"Something going wrong! \n {e.Message} " });
            }
        }

        [HttpPost]
        [Route("CreateProduct")]
        public async Task<IActionResult> CreateProduct(string name, decimal price, string description)
        {
            try
            {
                await _productService.CreateAsync(name, price, description);

                var response = new
                { Message = "Product created successfully!" };

                return Ok(response);
            }
            catch (SqlException e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { errorText = $"Something going wrong! \n {e.Message} " });
            }
        }

        [HttpPost]
        [Route("UpdateProduct")]
        public async Task<IActionResult> UpdateProduct([FromBody] ProductView productView)
        {
            try
            {
                await _productService.UpdateAsync(productView);
                var response = new
                { Message = "Updated successfully!" };
                return Ok(response);
            }
            catch (SqlException e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { errorText = $"Something going wrong! \n {e.Message} " });
            }
        }

        [HttpPost]
        [Route("DeleteProduct")]
        public async Task<IActionResult> DeleteProductById(int id)
        {
            try
            {
                var userId = Guid.NewGuid();
                await _productService.RemoveByIdAsync(id, userId);

                var response = new
                { Message = "Deleted successfully!" };
                return Ok(response);

            }
            catch (SqlException e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { errorText = $"Something going wrong! \n {e.Message} " });
            }
        }

        [HttpPost]
        [Route("AddCategory")]
        public async Task<IActionResult> AddCategory(int categoryId, int productId)
        {
            try
            {
                await _productService.AddCategoryAsync(categoryId, productId);

                var response = new
                { Message = "Added successfully!" };
                return Ok(response);
            }
            catch (SqlException e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { errorText = $"Something going wrong! \n {e.Message} " });
            }
        }

        [HttpPost]
        [Route("RemoveCategory")]
        public async Task<IActionResult> RemoveCategory(int productId, int categoryId)
        {
            try
            {
                await _productService.RemoveCategoryAsync(productId, categoryId);

                var response = new
                { Message = "Deleted successfully!" };
                return Ok(response);
            }
            catch (SqlException e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { errorText = $"Something going wrong! \n {e.Message} " });
            }
        }
    }
}