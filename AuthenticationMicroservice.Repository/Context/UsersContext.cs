using AuthenticationMicroservice.Domain.Entities;
using AuthenticationMicroservice.Repository.Context.Configurations;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AuthenticationMicroservice.Repository.Context
{
    public class UsersContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        public UsersContext(DbContextOptions<UsersContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new UserConfiguration());
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