using CatalogMicroservice.Service.Services.CategoryService;
using CatalogMicroservice.Service.Services.CategoryService.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;

namespace CatalogMicroservice.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService service)
        {
            _categoryService = service;
        }

        [HttpGet]
        [Route("Categories")]
        public async Task<IActionResult> GetCategories()
        {
            try
            {
                var categories = await _categoryService.GetCategoriesAsync();

                return categories != null
                    ? Ok(categories)
                    : StatusCode(StatusCodes.Status404NotFound, new { errorText = "Not found categories" });
            }
            catch (SqlException e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { errorText = $"Something going wrong! \n {e.Message} " });
            }
        }

        [HttpPost]
        [Route("GetCategoryById")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var category = await _categoryService.GetCategoryByIdAsync(id);

                return category != null ? Ok(category) : StatusCode(StatusCodes.Status404NotFound, new { errorText = "Not found categories" });
            }
            catch (SqlException e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { errorText = $"Something going wrong! \n {e.Message} " });
            }
        }

        [HttpPost]
        [Route("GetCategoryByName")]
        public async Task<IActionResult> GetByName(string name)
        {
            try
            {
                var categories = await _categoryService.GetByNameAsync(name);

                return categories != null ? Ok(categories) : StatusCode(StatusCodes.Status404NotFound, new { errorText = "No matching categories" });
            }
            catch (SqlException e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { errorText = $"Something going wrong! \n {e.Message} " });
            }
        }

        [HttpPost]
        [Route("GetChildCategory")]
        public async Task<IActionResult> GetChildCategory(CategoryView categoryView)
        {
            try
            {
                var categories = await _categoryService.GetChildCategoriesAsync(categoryView);

                return categories != null ? Ok(categories) : StatusCode(StatusCodes.Status404NotFound, new { errorText = "Not found child categories" });
            }
            catch (SqlException e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { errorText = $"Something going wrong! \n {e.Message} " });
            }
        }
    }
}