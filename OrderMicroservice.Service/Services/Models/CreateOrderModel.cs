using System;

namespace OrderMicroservice.Service.Services.Models
{
    public class CreateOrderModel
    {
        public Guid UserId { get; init; }

        public int CartId { get; set; }

        public int AddressId { get; set; }

        public decimal TotalPrice { get; set; }
    }
}