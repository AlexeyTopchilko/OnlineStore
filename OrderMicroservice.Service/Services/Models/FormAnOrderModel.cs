using System;
using System.ComponentModel.DataAnnotations;

namespace OrderMicroservice.Service.Services.Models
{
    public class FormAnOrderModel
    {
        [Required]
        public Guid UserId { get; init; }

        [Required]
        public int CartId { get; set; }

        [Required]
        public int AddressId { get; set; }
    }
}