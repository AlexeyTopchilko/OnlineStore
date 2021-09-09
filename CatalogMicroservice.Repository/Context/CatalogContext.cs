using System;
using System.Threading;
using System.Threading.Tasks;
using CatalogMicroservice.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CatalogMicroservice.Repository.Context
{
    public class CatalogContext : DbContext
    {
        public DbSet<Product> Products { get; set; }

        public DbSet<Category> Categories { get; set; }

        public DbSet<ProductsCategories> ProductsCategories { get; set; }

        public CatalogContext(DbContextOptions<CatalogContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProductsCategories>().HasKey(_ => new { _.CategoryId, _.ProductId });
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken token = default)
        {

            foreach (var entry in ChangeTracker.Entries())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.CurrentValues["AddedDate"] = DateTime.UtcNow;
                        break;

                    case EntityState.Modified:
                        entry.CurrentValues["UpdatedDate"] = DateTime.UtcNow;
                        break;

                    case EntityState.Deleted:
                        entry.State = EntityState.Modified;
                        entry.CurrentValues["DeletedDate"] = DateTime.UtcNow;
                        break;
                }
            }

            return base.SaveChangesAsync(acceptAllChangesOnSuccess, token);
        }
    }
}