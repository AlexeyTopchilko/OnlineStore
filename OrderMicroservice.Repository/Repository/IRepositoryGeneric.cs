using Microsoft.EntityFrameworkCore.ChangeTracking;
using OrderMicroservice.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace OrderMicroservice.Repository.Repository
{
    public interface IRepositoryGeneric<TEntity> where TEntity : BaseEntity
    {
        Task<IEnumerable<TEntity>> GetAllAsync();

        Task<IEnumerable<TEntity>> GetByPredicate(Expression<Func<TEntity, bool>> predicate);

        Task<TEntity> GetByIdAsync(int id);

        Task<EntityEntry<TEntity>> CreateAsync(TEntity entity);

        Task UpdateAsync(TEntity entity);

        Task DeleteAsync(TEntity entity);
    }
}