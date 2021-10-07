using AutoMapper;
using CatalogMicroservice.Domain.Entities;
using CatalogMicroservice.Repository.Models;
using CatalogMicroservice.Repository.Models.Enums;
using CatalogMicroservice.Repository.Models.ResponseModels;
using CatalogMicroservice.Repository.Repository;
using CatalogMicroservice.Service.Services.ProductService.Models;
using CatalogMicroservice.Service.Services.ProductService.Models.RequestModels;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CatalogMicroservice.Service.Services.ProductService
{
    public class ProductService : IProductService
    {
        private readonly IRepositoryGeneric<Product> _productRepository;
        private readonly IMapper _mapper;
        private readonly IRepositoryGeneric<ProductsCategories> _productsCategoriesRepository;

        public ProductService(IRepositoryGeneric<Product> productRepository, IMapper mapper, IRepositoryGeneric<ProductsCategories> productsCategoriesRepository)
        {
            _productsCategoriesRepository = productsCategoriesRepository;
            _productRepository = productRepository;
            _mapper = mapper;
        }

        public async Task CreateAsync(string name, decimal price, string description)
        {
            Product product = new()
            {
                Name = name,
                Description = description,
                Price = price
            };
            await _productRepository.CreateAsync(product);
        }

        private async Task<ProductsResponse<Product>> GetByCategoryAsync(int sortMode, int skip, int take, int? categoryId)
        {
            var sortProps = SetSortProps(sortMode, skip, take);
            var products = await _productRepository.GetByPredicateWithSortAsync(_ => _.ProductsCategories.Any(_ => _.CategoryId == categoryId) && _.DeletedDate == null, sortProps);

            return products;
        }

        private async Task<ProductsResponse<Product>> GetByNameAsync(int sortMode, int skip, int take, string name)
        {
            var sortProps = SetSortProps(sortMode, skip, take);
            var products = await _productRepository.GetByPredicateWithSortAsync(_ => _.Name.Contains(name) && _.DeletedDate == null, sortProps);

            return products;
        }

        public async Task<ProductView> GetProductByIdAsync(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);

            return product != null && product.DeletedDate == null ? _mapper.Map<ProductView>(product) : null;
        }

        public async Task<ProductsView> GetProductsAsync(GetProductsRequestModel model)
        {
            if (model.CategoryId == null && model.Name == null)
            {
                var sortProps = SetSortProps(model.SortMode, model.Skip, model.Take);
                var products = await _productRepository.GetByPredicateWithSortAsync(_ => _.DeletedDate == null, sortProps);

                return products.Products.Any() ? _mapper.Map<ProductsView>(products) : null;
            }
            if (model.CategoryId != null)
            {
                var products = await GetByCategoryAsync(model.SortMode, model.Skip, model.Take, model.CategoryId);
                return products.Products != null ? _mapper.Map<ProductsView>(products) : null;
            }
            if (model.Name != null)
            {
                var products = await GetByNameAsync(model.SortMode, model.Skip, model.Take, model.Name);
                return products.Products != null ? _mapper.Map<ProductsView>(products) : null;
            }
            return null;
        }

        public async Task<bool> IsProductExistAsync(string productName)
        {
            return (await _productRepository.GetByPredicateAsync(_ => _.Name.Equals(productName, StringComparison.OrdinalIgnoreCase))).Any();
        }

        public async Task RemoveAsync(Product product, Guid userId)
        {
            await _productRepository.DeleteAsync(product, userId);
        }

        public async Task RemoveByIdAsync(int id, Guid userId)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product != null && product.DeletedDate == null)
                await _productRepository.DeleteAsync(product, userId);
        }

        public async Task UpdateAsync(ProductView productView)
        {
            if (productView != null)
            {
                productView.Categories = null;
                var product = _mapper.Map<Product>(productView);
                await _productRepository.UpdateAsync(product);
            }
        }

        public async Task AddCategoryAsync(int categoryId, int productId)
        {
            ProductsCategories productsCategories = new()
            {
                ProductId = productId,
                CategoryId = categoryId
            };
            if (!(await _productsCategoriesRepository
                .GetByPredicateAsync(_ => _.CategoryId == productsCategories.CategoryId && _.ProductId == productsCategories.ProductId))
                .Any())
                await _productsCategoriesRepository.CreateAsync(productsCategories);
        }

        public async Task RemoveCategoryAsync(int productId, int categoryId)
        {
            var productsCategories = (await _productsCategoriesRepository
                .GetByPredicateAsync(_ => _.ProductId == productId && _.CategoryId == categoryId))
                .FirstOrDefault();
            if (productsCategories != null)
                await _productsCategoriesRepository.DeleteAsync(productsCategories, Guid.NewGuid());
        }
        private static SortProps<Product, dynamic> SetSortProps(int sortMode, int skip, int take)
        {
            var sortProps = sortMode switch
            {
                (int)SortState.Default => new SortProps<Product, dynamic> { Direction = SortDirection.Ascending, Expression = _ => _.Id, Skip = skip, Take = take },

                (int)SortState.PriceAsc => new SortProps<Product, dynamic> { Direction = SortDirection.Ascending, Expression = _ => _.Price, Skip = skip, Take = take },

                (int)SortState.PriceDesc => new SortProps<Product, dynamic> { Direction = SortDirection.Descending, Expression = _ => _.Price, Skip = skip, Take = take },

                (int)SortState.ByName => new SortProps<Product, dynamic> { Direction = SortDirection.Ascending, Expression = _ => _.Name, Skip = skip, Take = take },

                _ => new SortProps<Product, dynamic> { Direction = SortDirection.Ascending, Expression = _ => _.Id, Skip = skip, Take = take },
            };

            return sortProps;
        }
    }
}