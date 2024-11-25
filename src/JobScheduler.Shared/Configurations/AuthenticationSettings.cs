using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace JobScheduler.Shared.Configurations;

public class AuthenticationSettings
{
        public string Audience { get; set; }
        public string Issuer { get; set; }
        public string Secret { get; set; }
        public string TokenName { get; set; }
        public int TokenLifeTime { get; set; }
        public int RefreshTokenSize { get; set; }

        public SymmetricSecurityKey GetKey() => 
                new(Encoding.UTF8.GetBytes(Secret));
}