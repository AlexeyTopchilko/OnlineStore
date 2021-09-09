using CatalogMicroservice.Domain.Entities;
using CatalogMicroservice.Service.Services.ProductService;
using CatalogMicroservice.Service.Services.ProductService.Models;
using CatalogMicroservice.Service.Services.ProductService.Models.RequestModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System;
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
        public async Task<IActionResult> GetProducts(int sortMode, int skip, int take)
        {
            try
            {
                GetProductsRequestModel model = new()
                {
                    SortMode = sortMode,
                    Skip = skip,
                    Take = take
                };
                var productsView = await _productService.GetProductsAsync(model);
                return productsView != null ? Ok(productsView) : StatusCode(StatusCodes.Status404NotFound);
            }
            catch (SqlException e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { errorText = $"Something going wrong! \n {e.Message} " });
            }
        }

        [HttpGet]
        [Route("ProductsByCategory")]
        public async Task<IActionResult> GetByCategory(int id, int sortMode, int skip, int take)
        {
            try
            {
                ProductsByCategoryRequestModel model = new()
                {
                    Id = id,
                    SortMode = sortMode,
                    Skip = skip,
                    Take = take
                };
                var productsViews = await _productService.GetByCategoryAsync(model);
                return productsViews != null ?
                    Ok(productsViews)
                    : StatusCode(StatusCodes.Status404NotFound, new { errorText = "No matching products!" });
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
                return product != null ? Ok(product) : StatusCode(StatusCodes.Status404NotFound, new { errorText = "No matching products" });
            }
            catch (SqlException e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { errorText = $"Something going wrong! \n {e.Message} " });
            }
        }

        [HttpGet]
        [Route("ProductsByName")]
        public async Task<IActionResult> GetByName(string name, int sortMode, int skip, int take)
        {
            try
            {
                ProductsByNameRequestModel model = new()
                {
                    Name = name,
                    SortMode = sortMode,
                    Skip = skip,
                    Take = take
                };
                var productsView = await _productService.GetByNameAsync(model);

                return productsView != null
                    ? Ok(productsView)
                    : StatusCode(StatusCodes.Status404NotFound, new { errorText = "Mo matching products" });
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
                Product product = new()
                {
                    Name = name,
                    Description = description,
                    Price = price
                };
                await _productService.CreateAsync(product);

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
                ProductsCategories productsCategories = new()
                {
                    ProductId = productId,
                    CategoryId = categoryId
                };
                await _productService.AddCategoryAsync(productsCategories);

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