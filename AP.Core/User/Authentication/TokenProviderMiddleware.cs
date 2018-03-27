using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using AP.Core.Model.User;
using System.Linq;

namespace AP.Core.User.Authentication
{
    public class TokenProviderMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly TokenProviderOptions _options;
        private readonly JsonSerializerSettings _serializerSettings;

        public TokenProviderMiddleware(
            RequestDelegate next,
            IOptions<TokenProviderOptions> options)
        {
            _next = next;

            _options = options.Value;
            ThrowIfInvalidOptions(_options);

            _serializerSettings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented
            };
        }

        public Task Invoke(HttpContext context)
        {
            if (!context.Request.Path.Equals(_options.Path, StringComparison.Ordinal))
            {
                return _next(context);
            }

            if (!context.Request.Method.Equals("POST"))
            {
                context.Response.StatusCode = 400;
                return context.Response.WriteAsync("Bad request.");
            }

            return GenerateToken(context);
        }

        private async Task GenerateToken(HttpContext context)
        {
            var serializer = new JsonSerializer();
            LoginInfo loginModel;

            using (var stream = new StreamReader(context.Request.Body))
            using (var jsonStream = new JsonTextReader(stream))
            {
                loginModel = (LoginInfo) serializer.Deserialize(jsonStream, typeof(LoginInfo));
            }

            if (loginModel == null)
            {
                context.Response.StatusCode = 400;
                await context.Response.WriteAsync("Username or Password is empty.");
                return;
            }

            var identity = await _options.IdentityResolver(loginModel);
            if (identity == null || identity.User == null)
            {
                context.Response.StatusCode = 400;
                await context.Response.WriteAsync("Invalid username or password.");
                return;
            }

            var now = System.DateTime.UtcNow;

            // Specifically add the jti (nonce), iat (issued timestamp), and sub (subject/user) claims.
            var basicClaims = new Claim[]
            {
                new Claim(JwtRegisteredClaimNames.Email, identity.User.Email),
                new Claim(JwtRegisteredClaimNames.Sub, loginModel.User),
                new Claim(JwtRegisteredClaimNames.Jti, _options.NonceGenerator()),
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
                issuer: _options.Issuer,
                audience: _options.Audience,
                claims: claims,
                notBefore: now,
                expires: now.Add(_options.Expiration),
                signingCredentials: _options.SigningCredentials);
            
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            //jwtTokenHandler.InboundClaimTypeMap.Clear();

            var encodedJwt = jwtTokenHandler.WriteToken(jwt);
            var response = new
            {
                access_token = encodedJwt,
                expires_in = (int)_options.Expiration.TotalSeconds
            };

            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonConvert.SerializeObject(response, _serializerSettings));
        }

        private static void ThrowIfInvalidOptions(TokenProviderOptions options)
        {
            if (string.IsNullOrEmpty(options.Path))
            {
                throw new ArgumentNullException(nameof(TokenProviderOptions.Path));
            }

            if (string.IsNullOrEmpty(options.Issuer))
            {
                throw new ArgumentNullException(nameof(TokenProviderOptions.Issuer));
            }

            if (string.IsNullOrEmpty(options.Audience))
            {
                throw new ArgumentNullException(nameof(TokenProviderOptions.Audience));
            }

            if (options.Expiration == TimeSpan.Zero)
            {
                throw new ArgumentException("Must be a non-zero TimeSpan.", nameof(TokenProviderOptions.Expiration));
            }

            if (options.IdentityResolver == null)
            {
                throw new ArgumentNullException(nameof(TokenProviderOptions.IdentityResolver));
            }

            if (options.SigningCredentials == null)
            {
                throw new ArgumentNullException(nameof(TokenProviderOptions.SigningCredentials));
            }

            if (options.NonceGenerator == null)
            {
                throw new ArgumentNullException(nameof(TokenProviderOptions.NonceGenerator));
            }
        }
    }
}
