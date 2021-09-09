using System.Collections.Generic;
using CatalogMicroservice.Domain.Entities;

namespace CatalogMicroservice.Service.Services.CategoryService.Models
{
    public class CategoryView : BaseEntity
    {
        public string Name { get; set; }

        public int? ParrentId { get; set; }

        public virtual IEnumerable<ProductDto> Products { get; set; }
    }
}