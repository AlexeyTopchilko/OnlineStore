namespace CartMicroservice.Domain.Entities
{
    public class CartProducts : BaseEntity
    {
        public int ProductId { get; set; }

        public int OrderId { get; set; }

        public virtual Cart Cart { get; set; }

        public int Quantity { get; set; }
    }
}