using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderMicroservice.Domain.Entities;
using OrderMicroservice.Domain.Enums;

namespace OrderMicroservice.Repository.Context.Configuration
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.HasKey(_ => _.Id);
            builder.Property(_ => _.Id).ValueGeneratedOnAdd();

            builder.Property(_ => _.AddressId).IsRequired();
            builder.Property(_ => _.CartId).IsRequired();
            builder.Property(_ => _.TotalPrice).IsRequired();
            builder.Property(_ => _.UserId).IsRequired();
            builder.Property(_ => _.State).HasDefaultValue(OrderStates.AwaitingPayment);
        }
    }
}