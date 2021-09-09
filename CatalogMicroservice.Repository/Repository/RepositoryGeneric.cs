using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using CatalogMicroservice.Domain.Entities;
using CatalogMicroservice.Repository.Context;
using CatalogMicroservice.Repository.Models;
using CatalogMicroservice.Repository.Models.Enums;
using CatalogMicroservice.Repository.Models.ResponseModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace CatalogMicroservice.Repository.Repository
{
    public class RepositoryGeneric<TEntity> : IRepositoryGeneric<TEntity> where TEntity : BaseEntity
    {
        private readonly CatalogContext _context;

        private readonly DbSet<TEntity> _dbSet;

        public RepositoryGeneric(CatalogContext context)
        {
            _context = context;
            _dbSet = context.Set<TEntity>();
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            var entities = await _dbSet.ToListAsync();
            return entities;
        }

        public async Task<IEnumerable<TEntity>> GetByPredicateAsync(Expression<Func<TEntity, bool>> predicate)
        {
            var query = await Task.Run(() => _dbSet.Where(predicate));
            var response = await query.ToListAsync();

            return response;
        }

        //Only for products!!!
        public async Task<ProductsResponse<TEntity>> GetByPredicateWithSortAsync<TKey>(Expression<Func<TEntity, bool>> predicate, SortProps<TEntity, TKey> sortProps)
        {
            var query = sortProps.Direction switch
            {
                SortDirection.Ascending => await Task.Run(() => _dbSet.Where(predicate).OrderBy(sortProps.Expression)
                .Skip(sortProps.Skip).Take(sortProps.Take)),

                SortDirection.Descending => await Task.Run(() => _dbSet.Where(predicate).OrderByDescending(sortProps.Expression)
                .Skip(sortProps.Skip).Take(sortProps.Take)),

                _ => await Task.Run(() => _dbSet.Where(predicate).OrderBy(sortProps.Expression)
                .Skip(sortProps.Skip).Take(sortProps.Take)),
            };
            var products = await query.ToListAsync();
            var count = _dbSet.Where(predicate).Count();
            var totalPages = count == 0 ? 1 : (count % sortProps.Take == 0 ? count / sortProps.Take : count / sortProps.Take + 1);

            var response = new ProductsResponse<TEntity>()
            {
                Products = products,
                TotalPages = totalPages
            };

            return response;
        }

        public async Task<TEntity> GetByIdAsync(int id)
        {
            var entity = await _dbSet.FirstOrDefaultAsync(_ => _.Id == id);
            return entity;
        }

        public async Task<EntityEntry> CreateAsync(TEntity entity)
        {
            if (entity == null) return null;

            var entryEntity = await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entryEntity;
        }

        public async Task UpdateAsync(TEntity entity)
        {
            await Task.Run(() => _dbSet.Update(entity));
            await _context.SaveChangesAsync();
        }
        public async Task DeleteAsync(TEntity entity, Guid userId)
        {
            entity.DeletedBy = userId;
            await Task.Run(() => _dbSet.Remove(entity));
            await _context.SaveChangesAsync();
        }
    }
}