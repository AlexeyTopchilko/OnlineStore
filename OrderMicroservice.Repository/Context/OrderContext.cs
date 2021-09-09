using Microsoft.EntityFrameworkCore;
using OrderMicroservice.Domain.Entities;
using OrderMicroservice.Repository.Context.Configuration;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace OrderMicroservice.Repository.Context
{
    public class OrderContext : DbContext
    {
        public DbSet<Order> Orders { get; set; }

        public OrderContext(DbContextOptions<OrderContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new OrderConfiguration());
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