using System.ComponentModel.DataAnnotations;

namespace AuthenticationMicroservice.Service.Models
{
    public class RegisterModel
    {
        [Required(ErrorMessage = "User Name is required")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [RegularExpression(@"^([\w.%+-]+)@([\w-]+\.)+([\w]{2,})$", ErrorMessage = "Email is not valid")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
    }
}