using AP.Core.Model.User;
using AP.Core.User.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace AP.Core.User.Authentication
{
    public class JwtTokenProducer
    {
        public static JwtResponse Produce(JwtIdentity identity, TokenProviderOptions options)
        {
            var now = System.DateTime.UtcNow;

            // Specifically add the jti (nonce), iat (issued timestamp), and sub (subject/user) claims.
            var basicClaims = new Claim[]
            {
                new Claim(JwtRegisteredClaimNames.Email, identity.User.Email),
                new Claim(JwtRegisteredClaimNames.Sub, identity.User.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, options.NonceGenerator()),
                new Claim(JwtRegisteredClaimNames.Iat, new DateTimeOffset(now).ToUniversalTime().ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
            };

            var claims = basicClaims;
            if (identity.Roles != null && identity.Roles.Any())
            {
                var roleClaims = identity.Roles.Select(x => new Claim(ClaimTypes.Role, x)).ToArray();
                claims = basicClaims.Concat(roleClaims).ToArray();
            }

            // Create the JWT and write it to a string
            var jwt = new JwtSecurityToken(
                issuer: options.Issuer,
                audience: options.Audience,
                claims: claims,
                notBefore: now,
                expires: now.Add(options.Expiration),
                signingCredentials: options.SigningCredentials);

            var jwtTokenHandler = new JwtSecurityTokenHandler();
            //jwtTokenHandler.InboundClaimTypeMap.Clear();

            var encodedJwt = jwtTokenHandler.WriteToken(jwt);

            if (identity.RefreshToken == null)
            {
                return null;
            }

            var response = new JwtResponse
            {
                AccessToken = encodedJwt,
                ExpiresIn = (int)options.Expiration.TotalSeconds,
                RefreshToken = identity.RefreshToken.Token
            };

            return response;
        }

        public static TokenProviderOptions InitializeOptions(IConfigurationRoot config)
        {
            var signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(config.GetSection("TokenAuthentication:SecretKey").Value));

            var tokenProviderOptions = new TokenProviderOptions
            {
                Path = config.GetSection("TokenAuthentication:TokenPath").Value,
                RefreshTokenPath = config.GetSection("TokenAuthentication:RefreshTokenPath").Value,
                Audience = config.GetSection("TokenAuthentication:Audience").Value,
                Issuer = config.GetSection("TokenAuthentication:Issuer").Value,
                SigningCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256),
            };

            return tokenProviderOptions;
        }
    }
}
