using AP.Core.Model.User;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Threading.Tasks;

namespace AP.Core.User.Authorization
{
    public class TokenProviderOptions
    {
        public string Path { get; set; } = "/token";
        public string RefreshTokenPath { get; set; }
        /// <summary>
        ///  The Issuer (iss) claim for generated tokens.
        /// </summary>
        public string Issuer { get; set; }

        /// <summary>
        /// The Audience (aud) claim for the generated tokens.
        /// </summary>
        public string Audience { get; set; }

        public TimeSpan Expiration { get; set; } = TimeSpan.FromMinutes(5);

        /// <summary>
        /// The signing key to use when generating tokens.
        /// </summary>
        public SigningCredentials SigningCredentials { get; set; }
        public Func<LoginInfo, Task<JwtIdentity>> IdentityResolver { get; set; }
        public Func<string, Task<JwtIdentity>> RefreshTokenResolver { get; set; }
        public Func<string> NonceGenerator { get; set; } = () => Guid.NewGuid().ToString();
    }
}
