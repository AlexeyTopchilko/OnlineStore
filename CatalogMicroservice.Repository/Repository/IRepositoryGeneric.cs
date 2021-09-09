using CatalogMicroservice.Domain.Entities;
using CatalogMicroservice.Repository.Models;
using CatalogMicroservice.Repository.Models.ResponseModels;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace CatalogMicroservice.Repository.Repository
{
    public interface IRepositoryGeneric<TEntity> where TEntity : BaseEntity
    {
        Task<IEnumerable<TEntity>> GetAllAsync();

        Task<IEnumerable<TEntity>> GetByPredicateAsync(Expression<Func<TEntity, bool>> predicate);

        Task<ProductsResponse<TEntity>> GetByPredicateWithSortAsync<TKey>(Expression<Func<TEntity, bool>> predicate, SortProps<TEntity, TKey> sortProps);

        Task<TEntity> GetByIdAsync(int id);

        Task<EntityEntry> CreateAsync(TEntity entity);

        Task UpdateAsync(TEntity entity);

        Task DeleteAsync(TEntity entity, Guid Id);
    }
}