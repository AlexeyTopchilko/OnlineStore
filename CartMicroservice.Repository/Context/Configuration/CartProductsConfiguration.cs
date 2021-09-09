using CartMicroservice.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CartMicroservice.Repository.Context.Configuration
{
    public class CartProductsConfiguration : IEntityTypeConfiguration<CartProducts>
    {
        public void Configure(EntityTypeBuilder<CartProducts> builder)
        {
            builder.HasKey(_ => new { _.OrderId, _.ProductId });
            builder.HasOne(_ => _.Cart).WithMany(_ => _.Products).HasForeignKey(_ => _.OrderId);
            builder.HasAlternateKey(_ => _.Id);
            builder.Property(_ => _.Id).ValueGeneratedOnAdd();
            builder.Property(_ => _.Quantity).IsRequired();
        }
    }
}