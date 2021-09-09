using System;
using System.Collections.Generic;

namespace CartMicroservice.Domain.Entities
{
    public class Cart : BaseEntity
    {
        public virtual IEnumerable<CartProducts> Products { get; set; }

        public Guid UserId { get; init; }

        public decimal? TotalPrice { get; set; }

        public bool Locked { get; set; }
    }
}