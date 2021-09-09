using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace AuthenticationMicroservice.Service.Services
{
    public class AuthOptions
    {
        public const string Issuer = "God";
        public const string Audience = "Clients";
        const string Key = "SuperSecretKey123";
        public const int Lifetime = 60;
        public static SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Key));
        }
    }
}