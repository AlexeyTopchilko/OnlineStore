using CatalogMicroservice.Domain.Entities;
using CatalogMicroservice.Service.Services.CategoryService.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CatalogMicroservice.Service.Services.CategoryService
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryView>> GetCategoriesAsync();

        Task<CategoryView> GetCategoryByIdAsync(int id);

        Task AddAsync(Category category);

        Task RemoveAsync(Category category, Guid Id);

        Task RemoveByIdAsync(int id, Guid Id);

        Task UpdateAsync(Category category);

        Task<IEnumerable<CategoryView>> GetByNameAsync(string name);

        Task<IEnumerable<CategoryView>> GetChildCategoriesAsync(CategoryView categoryView);
    }
}