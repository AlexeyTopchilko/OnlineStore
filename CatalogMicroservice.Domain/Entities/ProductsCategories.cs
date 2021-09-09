namespace CatalogMicroservice.Domain.Entities
{
    public class ProductsCategories : BaseEntity
    {
        public int ProductId { get; set; }

        public virtual Product Product { get; set; }

        public int CategoryId { get; set; }

        public virtual Category Category { get; set; }
    }
}