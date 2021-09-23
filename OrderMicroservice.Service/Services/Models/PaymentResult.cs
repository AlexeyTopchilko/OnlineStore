namespace OrderMicroservice.Service.Services.Models
{
    public class PaymentResult
    {
        public int OrderId { get; set; }

        public bool Paid { get; set; }
    }
}