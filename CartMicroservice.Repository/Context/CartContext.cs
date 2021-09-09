using CartMicroservice.Domain.Entities;
using CartMicroservice.Repository.Context.Configuration;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CartMicroservice.Repository.Context
{
    public class CartContext : DbContext
    {
        public DbSet<Cart> Carts { get; set; }

        public DbSet<CartProducts> CartProducts { get; set; }

        public CartContext(DbContextOptions<CartContext> options)
            : base(options)
        {
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new CartConfiguration());
            modelBuilder.ApplyConfiguration(new CartProductsConfiguration());
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