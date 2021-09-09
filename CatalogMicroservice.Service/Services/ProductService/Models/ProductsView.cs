using System.Collections.Generic;

namespace CatalogMicroservice.Service.Services.ProductService.Models
{
    public class ProductsView
    {
        public IEnumerable<ProductView> Products { get; set; }

        public int TotalPages { get; set; }
    }
}