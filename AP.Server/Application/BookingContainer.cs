using AP.Business.AutoPortal.Order;
using AP.Business.Registration;
using AP.Business.Registration.Handlers;
using AP.Business.Registrations.Handlers;
using AP.Infrastructure;
using AP.Infrastructure.Messaging.Handling;
using AP.Infrastructure.Serialization;
using AP.Registration.Handlers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;

namespace AP.Server.Application
{
    public sealed partial class BookingContainer
    {
        private CancellationTokenSource cancellationTokenSource;
        private List<IProcessor> processors;

        public BookingContainer()
        {
            cancellationTokenSource = new CancellationTokenSource();
        }
        public void CreateContainer(IServiceCollection services, ILoggerFactory loggerFactory)
        {
            var serializer = new JsonTextSerializer();
            services.AddSingleton<ITextSerializer>(serializer);

            var metadataProvider = new StandardMetadataProvider();
            services.AddSingleton<IMetadataProvider>(metadataProvider);

            //services.AddScoped(provider =>
            //{
            //    Func<string, ICommandHandler> accessor = key =>
            //    {
            //        switch (key)
            //        {
            //            case "RegistrationProcessManagerRouter":
            //                return provider.GetService<RegistrationProcessManagerRouter>();
            //            case "OrderCommandHandler":
            //                return provider.GetService<OrderCommandHandler>();
            //            case "AnchorsAvailabilityHandler":
            //                return provider.GetService<AnchorsAvailabilityHandler>();
            //            case "AnchorAssignmentsHandler":
            //                return provider.GetService<AnchorAssignmentsHandler>();
            //            default:
            //                throw new KeyNotFoundException(string.Format("Key is {0}.", key));
            //        }
            //    };

            //    return accessor;
            //});

            services.AddScoped<ICommandHandler, RegistrationProcessManagerRouter>();
            services.AddScoped<ICommandHandler, OrderCommandHandler>();
            services.AddScoped<ICommandHandler, AnchorsAvailabilityHandler>();
            //services.AddScoped<ICommandHandler, ThirdPartyProcessorPaymentCommandHandler>("ThirdPartyProcessorPaymentCommandHandler");
            services.AddScoped<ICommandHandler, AnchorAssignmentsHandler>();

            services.AddSingleton<RegistrationProcessManagerRouter>();
            services.AddSingleton<PricedOrderViewModelGenerator>();
            services.AddSingleton<WorkshopViewModelGenerator>();
            services.AddSingleton<DraftOrderViewModelGenerator>();
            services.AddSingleton<AnchorAssignmentsViewModelGenerator>();
            services.AddSingleton<AnchorAssignmentsHandler>();
            services.AddSingleton<OrderEventHandler>();
            services.AddSingleton<IPricingService, PricingService>();

            // Conference management integration
            //container.RegisterType<global::Conference.ConferenceContext>(new TransientLifetimeManager(), new InjectionConstructor("ConferenceManagement"));

            OnCreateContainer(services, serializer, metadataProvider, loggerFactory);
        }

        partial void OnCreateContainer(IServiceCollection services, ITextSerializer serializer, IMetadataProvider metadataProvider, ILoggerFactory loggerFactory);
    }
}
