using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace AuthenticationMicroservice.Repository.Repository
{
    public interface IRepositoryGeneric<TEntity> where TEntity : class
    {
        Task<IEnumerable<TEntity>> GetAllAsync();

        Task<IEnumerable<TEntity>> GetByPredicate(Expression<Func<TEntity, bool>> predicate);

        Task<TEntity> GetByIdAsync(Guid id);

        Task CreateAsync(TEntity entity);

        Task UpdateAsync(TEntity entity);

        Task DeleteAsync(TEntity entity, Guid id);
    }
}