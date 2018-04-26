using AP.Core.Model.User;
using AP.Core.User.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace AP.Core.User.Authentication
{
    public class JwtTokenProducer
    {
        private static string _iosRoleClaim = "role";
        private static string _iosPermissionsClaim = "permissions";
        
        public static JwtResponse Produce(JwtIdentity identity, TokenProviderOptions options)
        {
            var now = System.DateTime.UtcNow;

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Email, identity.User.Email),
                new Claim(JwtRegisteredClaimNames.Sub, identity.User.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, options.NonceGenerator()),
                new Claim(JwtRegisteredClaimNames.Iat, new DateTimeOffset(now).ToUniversalTime().ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
            };

            if (identity.Claims != null)
            {
                claims.AddRange(identity.Claims);
                claims.Add(AddIosSpecificClaims(identity.Claims));
            }

            if (identity.Roles != null && identity.Roles.Any())
            {
                var roleClaims = identity.Roles.Select(x => new Claim(ClaimTypes.Role, x));
                claims.AddRange(roleClaims);
                claims.Add(AddIosSpecificRoles(identity.Roles));
            }

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

        private static Claim AddIosSpecificClaims(IEnumerable<Claim> claims)
        {
            return new Claim(_iosPermissionsClaim, claims.LastOrDefault().Value);
        }

        private static Claim AddIosSpecificRoles(IEnumerable<string> roles)
        {
            return new Claim(_iosRoleClaim, roles.FirstOrDefault()); 
        }

        public static TokenProviderOptions InitializeOptions(IConfigurationRoot config)
        {
            var signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(config.GetSection("TokenAuthentication:SecretKey").Value));
            var expirationMins = int.Parse(config.GetSection("TokenAuthentication:ExpMinOauth").Value);

            var tokenProviderOptions = new TokenProviderOptions
            {
                Path = config.GetSection("TokenAuthentication:TokenPath").Value,
                RefreshTokenPath = config.GetSection("TokenAuthentication:RefreshTokenPath").Value,
                Audience = config.GetSection("TokenAuthentication:Audience").Value,
                Issuer = config.GetSection("TokenAuthentication:Issuer").Value,
                SigningCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256),
                Expiration = TimeSpan.FromMinutes(expirationMins)
            };

            return tokenProviderOptions;
        }
    }
}
