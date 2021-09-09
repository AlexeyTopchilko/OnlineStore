using AddressMicroservice.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AddressMicroservice.Repository.Context.Configuration
{
    public class AddressConfiguration : IEntityTypeConfiguration<Address>
    {
        public void Configure(EntityTypeBuilder<Domain.Entities.Address> builder)
        {
            builder.HasKey(_ => _.Id);
            builder.Property(_ => _.Id).ValueGeneratedOnAdd();

            builder.Property(_ => _.City).IsRequired();
            builder.Property(_ => _.HouseNumber).IsRequired();
            builder.Property(_ => _.Street).IsRequired();
            builder.Property(_ => _.UserId).IsRequired();
        }
    }
}