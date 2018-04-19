using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using AP.Server.Application;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using AP.Core.User.Authorization;
using System;
using Microsoft.ApplicationInsights;
using System.Net;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using AP.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Security.Claims;
using AP.Core.Model.User;
using Swashbuckle.AspNetCore.Swagger;

namespace AP.Server
{
    public partial class Startup
    {
        private TelemetryClient _telemetryClient = new TelemetryClient();
        private BookingContainer bookingContainer = new BookingContainer();
        private ILoggerFactory loggerFactory;

        public Startup(IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            try
            {
                var builder = new ConfigurationBuilder()
                    .SetBasePath(env.ContentRootPath)
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                    .AddEnvironmentVariables()
                    .AddUserSecrets<Startup>();
                    // Add the EF configuration provider, which will override any
                    // config made with the JSON provider.
                    /*
                    .AddEntityFrameworkConfig(options =>
                        options.UseSqlServer(connectionStringConfig.GetConnectionString(
                        "DefaultConnection"))
                    )*/

                Configuration = builder.Build();
                this.loggerFactory = loggerFactory;
            }
            catch(Exception ex)
            {
                _telemetryClient.TrackException(ex);
                throw new Exception("msg_id", ex);
            }
        }
        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            try
            {
                services.AddAuthorization(options =>
                {
                    options.AddPolicy("Bearer", new AuthorizationPolicyBuilder()
                        .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                        .RequireAuthenticatedUser()
                        .Build()
                    );

                    options.AddPolicy("Verified", policy => policy.RequireClaim(ClaimTypes.AuthorizationDecision, 
                        Enum.GetName(typeof(ApplicationClaims), ApplicationClaims.Verified)));
                    options.AddPolicy("Accomplished", policy => policy.RequireClaim(ClaimTypes.AuthorizationDecision,
                        Enum.GetName(typeof(ApplicationClaims), ApplicationClaims.Accomplished)));
                });

                services.AddMvc()
                        .AddDataAnnotationsLocalization(options => {
                            options.DataAnnotationLocalizerProvider = (type, factory) =>
                                factory.Create(typeof(Shared.Resources.Annotations));
                        });

                services.AddMemoryCache();
                services.AddRouting();

                //TODO:put configure identity here
                ConfigureIdentity(services);

                loggerFactory
                    .AddConsole(Configuration.GetSection("Logging"))
                    .AddDebug();

                services.AddLogging();

                services.AddSwaggerGen(c =>
                {
                    c.SwaggerDoc("v1", new Info { Title = "My API", Version = "v1" });
                });

                DiContainer.RegisterScopes(services, Configuration);
                bookingContainer.CreateContainer(services, loggerFactory);
            }
            catch(Exception ex)
            {
                _telemetryClient.TrackException(ex);
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler(options =>
                {
                    options.Run(
                        async context =>
                        {
                            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                            context.Response.ContentType = "text/html";
                            var ex = context.Features.Get<IExceptionHandlerFeature>();
                            if (ex != null)
                            {
                                var err = $"<h1>Error: {ex.Error.Message}</h1>{ex.Error.StackTrace }";
                                await context.Response.WriteAsync(err).ConfigureAwait(false);
                            }
                        });
                });
            }

            ConfigureAuth(app, env);
            //app.UseIdentity();

            app.UseMvc();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });

            StartListen(app.ApplicationServices);
            StartupRoles.Create(app.ApplicationServices).Wait();

            AutomapperConfig.RegisterModels();
        }

        private static void StartListen(IServiceProvider provider)
        {
            var processors = provider.GetServices<IProcessor>();
            foreach (var proc in processors)
            {
                proc.Start();
            }
        }
    }
}
