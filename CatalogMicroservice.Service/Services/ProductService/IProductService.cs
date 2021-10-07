using CatalogMicroservice.Domain.Entities;
using CatalogMicroservice.Service.Services.ProductService.Models;
using CatalogMicroservice.Service.Services.ProductService.Models.RequestModels;
using System;
using System.Threading.Tasks;

namespace CatalogMicroservice.Service.Services.ProductService
{
    public interface IProductService
    {
        Task<ProductsView> GetProductsAsync(GetProductsRequestModel model);

        Task<ProductView> GetProductByIdAsync(int id);

        Task CreateAsync(string name, decimal price, string description);

        Task<bool> IsProductExistAsync(string productName);

        Task RemoveAsync(Product product, Guid userId);

        Task RemoveByIdAsync(int id, Guid userId);

        Task UpdateAsync(ProductView productView);

        Task AddCategoryAsync(int categoryId, int productId);

        Task RemoveCategoryAsync(int productId, int categoryId);
    }
}