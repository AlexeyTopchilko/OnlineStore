using System;
using System.Collections.Generic;

namespace OrderMicroservice.Service.Services.Models
{
    public class OrderViewModel
    {
        public Guid UserId { get; init; }

        public IEnumerable<ProductViewModel> Products { get; set; }

        public AddressViewModel Address { get; set; }

        public decimal TotalPrice { get; set; }
    }
}