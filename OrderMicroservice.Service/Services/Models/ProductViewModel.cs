namespace OrderMicroservice.Service.Services.Models
{
    public class ProductViewModel
    {
        public int ProductId { get; set; }

        public string Name { get; set; }

        public decimal Price { get; set; }

        public string Image { get; set; }

        public int Quantity { get; set; }

        public decimal TotalPrice { get; set; }
    }
}