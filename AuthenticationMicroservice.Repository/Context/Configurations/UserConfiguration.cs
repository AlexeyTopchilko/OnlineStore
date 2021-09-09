using AuthenticationMicroservice.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthenticationMicroservice.Repository.Context.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(_ => _.Id);
            builder.Property(_ => _.Username).IsRequired().HasMaxLength(10);
            builder.Property(_ => _.Email).IsRequired().HasMaxLength(50);
            builder.Property(_ => _.Password).IsRequired().HasMaxLength(24);
            builder.Property(_ => _.Role).IsRequired();
        }
    }
}