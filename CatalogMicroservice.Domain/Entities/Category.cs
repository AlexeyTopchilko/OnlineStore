using System.Collections.Generic;

namespace CatalogMicroservice.Domain.Entities
{
    public class Category : BaseEntity
    {
        public string Name { get; set; }

        public int? ParrentId { get; set; }

        public virtual IList<ProductsCategories> ProductsCategories { get; set; }
    }
}