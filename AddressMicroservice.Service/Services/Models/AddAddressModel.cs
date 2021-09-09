using System;
using System.ComponentModel.DataAnnotations;

namespace AddressMicroservice.Service.Services.Models
{
    public class AddAddressModel
    {
        [Required]
        public Guid UserId { get; set; }

        [Required]
        public string City { get; set; }

        [Required]
        public string Street { get; set; }

        [Required]
        public string HouseNumber { get; set; }
    }
}