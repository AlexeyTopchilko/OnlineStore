using AuthenticationMicroservice.Domain.Entities;
using AuthenticationMicroservice.Repository.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace AuthenticationMicroservice.Repository.Repository
{
    public class RepositoryGeneric<TEntity> : IRepositoryGeneric<TEntity> where TEntity : BaseEntity
    {
        private readonly UsersContext _context;

        private readonly DbSet<TEntity> _dbSet;

        public RepositoryGeneric(UsersContext context)
        {
            _context = context;
            _dbSet = context.Set<TEntity>();
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<IEnumerable<TEntity>> GetByPredicate(Expression<Func<TEntity, bool>> predicate)
        {
            var query = await Task.Run(() => _dbSet.AsQueryable());

            var response = Task.Run(() => query.Where(predicate).ToList());

            return await response;
        }

        public async Task<TEntity> GetByIdAsync(Guid id)
        {
            return await _dbSet.FirstOrDefaultAsync(_ => _.Id == id);
        }

        public async Task CreateAsync(TEntity entity)
        {
            if (entity != null)
            {
                await _dbSet.AddAsync(entity);
                await _context.SaveChangesAsync();
            }
        }

        public async Task UpdateAsync(TEntity entity)
        {
            await Task.Run(() => _dbSet.Update(entity));
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(TEntity entity, Guid id)
        {
            entity.DeletedBy = id;
            await Task.Run(() => _dbSet.Remove(entity));
            await _context.SaveChangesAsync();
        }
    }
}