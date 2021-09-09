using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using OrderMicroservice.Domain.Entities;
using OrderMicroservice.Repository.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace OrderMicroservice.Repository.Repository
{
    public class RepositoryGeneric<TEntity> : IRepositoryGeneric<TEntity> where TEntity : BaseEntity
    {
        private readonly OrderContext _context;

        private readonly DbSet<TEntity> _dbSet;

        public RepositoryGeneric(OrderContext context)
        {
            _context = context;

            _dbSet = context.Set<TEntity>();
        }

        public async Task<EntityEntry<TEntity>> CreateAsync(TEntity entity)
        {
            var entryEntity = await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entryEntity;
        }

        public async Task DeleteAsync(TEntity entity)
        {
            await Task.Run(() => _dbSet.Remove(entity));
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            var entities = await _dbSet.ToListAsync();
            return entities;
        }

        public async Task<TEntity> GetByIdAsync(int id)
        {
            var entities = await _dbSet.FirstOrDefaultAsync(_ => _.Id == id && _.DeletedDate == null);
            return entities;
        }

        public async Task<IEnumerable<TEntity>> GetByPredicate(Expression<Func<TEntity, bool>> predicate)
        {
            var query = await Task.Run(() => _dbSet.Where(predicate));
            var response = await query.ToListAsync();

            return response;
        }

        public async Task UpdateAsync(TEntity entity)
        {
            await Task.Run(() => _dbSet.Update(entity));
            await _context.SaveChangesAsync();
        }
    }
}