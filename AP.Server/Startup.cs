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
using AP.Business.Registration;
using AP.Business.Registration.Handlers;
using AP.Registration.Handlers;
using AP.Infrastructure.Sql.MessageLog;
using AP.Business.Registrations.Handlers;
using AP.Business.AutoPortal.Order;
using AP.Infrastructure.Messaging.Handling;
using AP.Infrastructure;
using EntityFramework.DbContextScope.Interfaces;
using AP.Infrastructure.Messaging;
using Microsoft.Extensions.Caching.Memory;
using AP.Infrastructure.Serialization;
using AP.Infrastructure.Processes;
using AP.Infrastructure.EventSourcing;
using AP.Business.Registration.ReadModel;
using AP.Infrastructure.BlobStorage;
using AP.Repository.Booking.Contracts;

namespace AP.Server
{
    public class Startup
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
                services.AddMvc(config =>
                        {
                            var policy = new AuthorizationPolicyBuilder()
                                            .RequireAuthenticatedUser()
                                            .Build();
                            config.Filters.Add(new AuthorizeFilter(policy));
                        }
                    )
                    .AddDataAnnotationsLocalization(options => {
                        options.DataAnnotationLocalizerProvider = (type, factory) =>
                            factory.Create(typeof(Shared.Resources.Annotations));
                        });

                services.AddMemoryCache();
                services.AddRouting();

                DiContainer.RegisterScopes(services, Configuration);
                bookingContainer.CreateContainer(services, loggerFactory);

                AutomapperConfig.RegisterModels();
            }
            catch(Exception ex)
            {
                _telemetryClient.TrackException(ex);
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
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

            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseIdentity();

            app.UseMvc();

            //AppRoute.BuildRoutes(app);

            //RegisterSqlEventHandlers(app.ApplicationServices);
            //RegisterAzureEventHandlers(app.ApplicationServices);

            //RegisterCommandHandlers(app.ApplicationServices);

            //StartListen(app.ApplicationServices);
            StartupRoles.Create(app.ApplicationServices).Wait();
        }
        /*
        private void RegisterAzureEventHandlers(IServiceProvider provider)
        {
            //var busConfig = new ServiceBusConfig(azureSettings.ServiceBus, null);
            var serializer = provider.GetService<ITextSerializer>();
            RegisterEventProcessor<RegistrationProcessManagerRouter>(provider, busConfig, Topics.Events.Subscriptions.RegistrationPMNextSteps, serializer);
        }

        private void RegisterEventProcessor<T>(IServiceProvider provider, ServiceBusConfig busConfig, string subscriptionName, ITextSerializer serializer) where T : IEventHandler
        {
            var service = provider.GetService<T>();
            busConfig.CreateEventProcessor(Topics.Events.Subscriptions.RegistrationPMNextSteps, service, serializer);
        }
        */
        private static void RegisterSqlEventHandlers(IServiceProvider provider)
        {
            var eventProcessor = provider.GetService<IEventHandlerRegistry>();
            var ambientContext = provider.GetService<IAmbientDbContextLocator>();
            var factory = provider.GetService<IDbContextScopeFactory>();
            var draftLogger = provider.GetService<ILogger<DraftOrderViewModelGenerator>>();
            var workshopLogger = provider.GetService<ILogger<WorkshopViewModelGenerator>>();
            var orderLogger = provider.GetService<ILogger<OrderEventHandler>>();
            var commandBus = provider.GetService<ICommandBus>();
            var cache = provider.GetService<IMemoryCache>();
            var serializer = provider.GetService<ITextSerializer>();
            var messageLog = new SqlMessageLog(factory, ambientContext, serializer, provider.GetService<IMetadataProvider>());
            var sqlProcessManagerContext = provider.GetService<Func<IProcessManagerDataContext<RegistrationProcessManager>>>();
            var eventSourcedOrderRepo = provider.GetService<IEventSourcedRepository<Order>>();
            var eventSourcedAnchors = provider.GetService<IEventSourcedRepository<AnchorAssignments>>();
            var workshopDao = provider.GetService<IWorkshopDao>();
            var blob = provider.GetService<IBlobStorage>();
            var orderRepository = provider.GetService<IOrderRepository>();
            var registrationLogger = provider.GetService<ILogger<RegistrationProcessManagerRouter>>();

            eventProcessor.Register(new RegistrationProcessManagerRouter(sqlProcessManagerContext, registrationLogger));
            eventProcessor.Register(new DraftOrderViewModelGenerator(factory, draftLogger, ambientContext));
            eventProcessor.Register(new PricedOrderViewModelGenerator(factory, ambientContext, cache));
            eventProcessor.Register(new WorkshopViewModelGenerator(factory, commandBus, ambientContext, workshopLogger));
            eventProcessor.Register(new AnchorAssignmentsViewModelGenerator(workshopDao, blob, serializer));
            eventProcessor.Register(new AnchorAssignmentsHandler(eventSourcedOrderRepo, eventSourcedAnchors));
            eventProcessor.Register(new OrderEventHandler(factory, orderRepository, orderLogger));
            eventProcessor.Register(new SqlMessageLogHandler(messageLog));
        }

        private static void RegisterCommandHandlers(IServiceProvider provider)
        {
            var commandHandlerRegistry = provider.GetService<ICommandHandlerRegistry>();

            foreach (var commandHandler in provider.GetServices<ICommandHandler>())
            {
                commandHandlerRegistry.Register(commandHandler);
            }
        }

        private static void StartListen(IServiceProvider provider)
        {
            var command = provider.GetService<Func<string, IProcessor>>()("CommandProcessor");
            command.Start();
            var events = provider.GetService<Func<string, IProcessor>>()("EventProcessor");
            events.Start();
        }
    }
}
