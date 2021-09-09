using System;
using OrderMicroservice.Domain.Enums;

namespace OrderMicroservice.Domain.Entities
{
    public class Order : BaseEntity
    {
        public Guid UserId { get; init; }

        public int CartId { get; set; }

        public OrderStates State { get; set; }

        public int AddressId { get; set; }

        public decimal TotalPrice { get; set; }
    }
}