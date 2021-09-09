using CatalogMicroservice.Domain.Entities;
using System.Collections.Generic;

namespace CatalogMicroservice.Service.Services.ProductService.Models
{
    public class ProductView : BaseEntity
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public decimal Price { get; set; }

        public string Image { get; set; }

        public virtual List<CategoryDto> Categories { get; set; }
    }
}