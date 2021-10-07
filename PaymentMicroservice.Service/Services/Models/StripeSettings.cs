namespace PaymentMicroservice.Service.Services.Models
{
    public class StripeSettings
    {
        public string SecretKey { get; set; }

        public string PublishableKey { get; set; }
    }
}