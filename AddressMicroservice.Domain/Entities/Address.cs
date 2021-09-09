using System;

namespace AddressMicroservice.Domain.Entities
{
    public class Address : BaseEntity
    {
        public Guid UserId { get; set; }

        public string City { get; set; }

        public string Street { get; set; }

        public string HouseNumber { get; set; }
    }
}