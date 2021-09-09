using AuthenticationMicroservice.Domain.Enums;

namespace AuthenticationMicroservice.Domain.Entities
{
    public class User : BaseEntity
    {
        public string Username { get; set; }

        public string Password { get; set; }

        public string Email { get; set; }

        public Roles Role { get; set; }
    }
}