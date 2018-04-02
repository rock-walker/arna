using AP.Core.Model.User;
using AP.Core.User.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;

namespace AP.Core.User.Authorization
{
    public class RefreshTokenProviderMiddleware
    {
        private readonly RequestDelegate next;
        private readonly TokenProviderOptions options;
        private readonly JsonSerializerSettings serializerSettings;

        public RefreshTokenProviderMiddleware(
            RequestDelegate next,
            IOptions<TokenProviderOptions> options)
        {
            this.next = next;
            this.options = options.Value;

            this.serializerSettings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented
            };
        }

        public Task Invoke(HttpContext context)
        {
            if (!context.Request.Path.Equals(options.RefreshTokenPath, StringComparison.OrdinalIgnoreCase))
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
            RefreshToken refreshToken;

            using (var stream = new StreamReader(context.Request.Body))
            using (var jsonStream = new JsonTextReader(stream))
            {
                refreshToken = (RefreshToken)serializer.Deserialize(jsonStream, typeof(RefreshToken));
            }

            if (refreshToken == null || string.IsNullOrWhiteSpace(refreshToken.Token))
            {
                context.Response.StatusCode = 400;
                await context.Response.WriteAsync("User must relogin.");
                return;
            }

            JwtIdentity identity;
            try
            {
                identity = await options.RefreshTokenResolver(refreshToken.Token);
            }
            catch (Exception ex)
            {
                context.Response.StatusCode = 400;
                await context.Response.WriteAsync(ex.Message);
                return;
            }

            var user = identity.User;
            var token = JwtTokenProducer.Produce(identity, options);

            if (token == null)
            {
                context.Response.StatusCode = 400;
                await context.Response.WriteAsync("refreshToken is missing.");
                return;
            }

            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonConvert.SerializeObject(token, serializerSettings));
        }
    }
}
