using System.Collections.Generic;

namespace CatalogMicroservice.Domain.Entities
{
    public class Product : BaseEntity
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public decimal Price { get; set; }

        public virtual IList<ProductsCategories> ProductsCategories { get; set; }

        public string Image { get; set; }
    }
}