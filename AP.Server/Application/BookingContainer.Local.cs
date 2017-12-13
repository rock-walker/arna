using AP.Business.Registration;
using AP.Business.Registration.Handlers;
using AP.Infrastructure;
using AP.Infrastructure.BlobStorage;
using AP.Infrastructure.Messaging;
using AP.Infrastructure.Messaging.Handling;
using AP.Infrastructure.Serialization;
using AP.Infrastructure.Sql.BlobStorage;
using AP.Infrastructure.Sql.MessageLog;
using AP.Infrastructure.Sql.Messaging;
using AP.Infrastructure.Sql.Messaging.Handling;
using AP.Infrastructure.Sql.Messaging.Implementation;
using AP.Registration.Handlers;
using EntityFramework.DbContextScope.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace AP.Server.Application
{
    public class BookingContainer
    {
        private static string _localConnectionString = "Data Source=.\\sqlexpress;Initial Catalog=autoportaldb;Integrated Security=True";

        public static void SetupBookingHandlers(IServiceCollection services)
        {
            services.AddScoped<ICommandHandler, RegistrationProcessManagerRouter>();
            services.AddScoped<ICommandHandler, OrderCommandHandler>();
            services.AddScoped<ICommandHandler, AnchorsAvailabilityHandler>();
            //services.AddScoped<ICommandHandler, ThirdPartyProcessorPaymentCommandHandler>("ThirdPartyProcessorPaymentCommandHandler");
            services.AddScoped<ICommandHandler, AnchorAssignmentsHandler>();

            // Conference management integration
            //container.RegisterType<global::Conference.ConferenceContext>(new TransientLifetimeManager(), new InjectionConstructor("ConferenceManagement"));
        }

        public static void OnCreateContainer(IServiceCollection services)
        {
            var serializer = new JsonTextSerializer();
            services.AddSingleton<ITextSerializer>(serializer);

            var metadataProvider = new StandardMetadataProvider();
            services.AddSingleton<IMetadataProvider>(metadataProvider);
            services.AddSingleton<IBlobStorage, SqlBlobStorage>();

            var eventBus = new EventBus(new MessageSender(_localConnectionString, "SqlBus.Events"), serializer);
            var commandBus = new CommandBus(new MessageSender(_localConnectionString, "SqlBus.Commands"), serializer);

            var commandProcessor = new CommandProcessor(new MessageReceiver(_localConnectionString, "SqlBus.Commands"), serializer);
            var eventProcessor = new EventProcessor(new MessageReceiver(_localConnectionString, "SqlBus.Events"), serializer);

            services.AddSingleton<IEventBus>(eventBus);
            services.AddSingleton<ICommandBus>(commandBus);
            services.AddSingleton<ICommandHandlerRegistry>(commandProcessor);
            services.AddSingleton<IEventHandlerRegistry>(eventProcessor);

            services.AddSingleton(provider =>
            {
                Func<string, IProcessor> accessor = key =>
                {
                    switch (key)
                    {
                        case "CommandProcessor":
                            return commandProcessor;
                        case "EventProcessor":
                            return eventProcessor;
                        default:
                            throw new KeyNotFoundException(string.Format("current {0} key wasn't found in service collection", key));
                    }
                };
                return accessor;
            });

            // Event log database and handler.
            services.AddScoped(provider => new SqlMessageLog(provider.GetService<IDbContextScopeFactory>(), 
                provider.GetService<IAmbientDbContextLocator>(), serializer, metadataProvider));
            services.AddScoped<IEventHandler, SqlMessageLogHandler>();
            services.AddScoped<ICommandHandler, SqlMessageLogHandler>();

            services.AddSingleton<IPricingService, PricingService>();
        }
    }
}
