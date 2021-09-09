namespace CatalogMicroservice.Service.Services.RabbitMqService.Models
{
    public class ProductsRequestModel
    {
        public int Id { get; set; }

        public int ProductId { get; set; }

        public int Quantity { get; set; }
    }
}