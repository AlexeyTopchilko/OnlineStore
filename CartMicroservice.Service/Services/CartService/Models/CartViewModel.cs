using System;
using System.Collections.Generic;
using CartMicroservice.Service.Services.RabbitMqService.Models;

namespace CartMicroservice.Service.Services.CartService.Models
{
    public class CartViewModel
    {
        public int Id { get; set; }

        public IEnumerable<ProductsResponseModel> Products { get; set; }

        public Guid UserId { get; set; }

        public decimal? TotalPrice { get; set; }
    }
}