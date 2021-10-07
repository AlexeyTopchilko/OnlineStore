using System.ComponentModel.DataAnnotations;

namespace CatalogMicroservice.Service.Services.ProductService.Models.RequestModels
{
    public class GetProductsRequestModel
    {
        [Required]
        public int SortMode { get; set; }

        [Required]
        public int Skip { get; set; }

        [Required]
        public int Take { get; set; }

        public int? CategoryId { get; set; }

        [MaxLength(15)] public string Name { get; set; }
    }
}