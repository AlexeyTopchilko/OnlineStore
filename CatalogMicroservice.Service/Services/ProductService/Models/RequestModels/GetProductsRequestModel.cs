namespace CatalogMicroservice.Service.Services.ProductService.Models.RequestModels
{
    public class GetProductsRequestModel
    {
        public int SortMode { get; set; }

        public int Skip { get; set; }

        public int Take { get; set; }
    }
}