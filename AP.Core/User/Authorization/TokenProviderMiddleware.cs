using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using System.IO;
using AP.Core.Model.User;
using AP.Core.User.Authentication;

namespace AP.Core.User.Authorization
{
    public class TokenProviderMiddleware
    {
        private readonly RequestDelegate next;
        private readonly TokenProviderOptions options;
        private readonly JsonSerializerSettings serializerSettings;

        public TokenProviderMiddleware(
            RequestDelegate next,
            IOptions<TokenProviderOptions> options)
        {
            this.next = next;

            this.options = options.Value;
            ThrowIfInvalidOptions(this.options);

            serializerSettings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented
            };
        }

        public Task Invoke(HttpContext context)
        {
            if (!context.Request.Path.Equals(options.Path, StringComparison.Ordinal))
            {
                return next(context);
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

            var identity = await options.IdentityResolver(loginModel);
            if (identity == null || identity.User == null)
            {
                context.Response.StatusCode = 400;
                await context.Response.WriteAsync("Invalid username or password.");
                return;
            }

            var token = JwtTokenProducer.Produce(identity, options);

            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonConvert.SerializeObject(token, serializerSettings));
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
