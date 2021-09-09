namespace CatalogMicroservice.Service.Services.ProductService.Models.RequestModels
{
    public class ProductsByNameRequestModel : GetProductsRequestModel
    {
        public string Name { get; set; }
    }
}