using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CatalogMicroservice.Domain.Entities;
using CatalogMicroservice.Repository.Models;
using CatalogMicroservice.Repository.Models.Enums;
using CatalogMicroservice.Repository.Repository;
using CatalogMicroservice.Service.Services.CategoryService.Models;

namespace CatalogMicroservice.Service.Services.CategoryService
{
    public class CategoryService : ICategoryService
    {
        private readonly IRepositoryGeneric<Category> _categoryRepository;
        private readonly IMapper _mapper;

        public CategoryService(IRepositoryGeneric<Category> categoryRepository, IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        public async Task AddAsync(Category category)
        {
            if (!await IsCategoryExistAsync(category.Name))
                await _categoryRepository.CreateAsync(category);
        }

        public async Task<IEnumerable<CategoryView>> GetByNameAsync(string name)
        {
            var categories =
                (await _categoryRepository.GetByPredicateAsync(_ => _.Name.Contains(name) && _.DeletedDate == null));

            return categories.Any() ? _mapper.Map<IEnumerable<CategoryView>>(categories) : null;
        }

        public async Task<IEnumerable<CategoryView>> GetCategoriesAsync()
        {
            var categories = await _categoryRepository.GetAllAsync();

            return categories != null ? _mapper.Map<IEnumerable<CategoryView>>(categories) : null;
        }

        public async Task<CategoryView> GetCategoryByIdAsync(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);

            return category != null && category.DeletedDate == null ? _mapper.Map<CategoryView>(category) : null;
        }

        public async Task<IEnumerable<CategoryView>> GetChildCategoriesAsync(CategoryView categoryView)
        {
            var categories =
                await _categoryRepository.GetByPredicateAsync(_ =>
                    _.ParrentId == categoryView.Id && _.DeletedDate == null);

            return categories.Any() ? _mapper.Map<IEnumerable<CategoryView>>(categories) : null;
        }

        public async Task RemoveAsync(Category category, Guid id)
        {
            await _categoryRepository.DeleteAsync(category, id);
        }

        public async Task RemoveByIdAsync(int id, Guid userId)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category != null)
                await _categoryRepository.DeleteAsync(category, userId);
        }

        public async Task UpdateAsync(Category category)
        {
            await _categoryRepository.UpdateAsync(category);
        }

        private static SortProps<Category, dynamic> SetSortProps()
        {
            var sortProps = new SortProps<Category, dynamic>()
            { Direction = SortDirection.Ascending, Expression = _ => _.Name };
            return sortProps;
        }

        private async Task<bool> IsCategoryExistAsync(string categoryName)
        {
            return (await _categoryRepository.GetByPredicateAsync(_ => _.Name == categoryName)).Any();
        }
    }
}