namespace OrderMicroservice.Service.Services.Models
{
    public class AddressViewModel
    {
        public int Id { get; set; }
        public string City { get; set; }

        public string Street { get; set; }

        public string HouseNumber { get; set; }
    }
}