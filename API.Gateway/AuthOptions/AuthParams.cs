using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace API.Gateway.AuthOptions
{
    public class AuthParams
    {
        public string Issuer { get; set; }

        public string Audience { get; set; }

        public string Key { get; set; }

        public string Lifetime { get; set; }

        public string ValidateAudience { get; set; }

        public string ValidateLifetime { get; set; }

        public string ValidateIssuerSigningKey { get; set; }

        public string ValidateIssuer { get; set; }

        public SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Key));
        }
    }
}