using System;

namespace CartMicroservice.Service.Services.CartService.Models
{
    public class AddToCartModel
    {
        public Guid UserId { get; set; }

        public int Id { get; set; }

        public int Quantity { get; set; }
    }
}