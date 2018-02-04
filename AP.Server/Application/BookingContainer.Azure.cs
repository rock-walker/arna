using AP.Business.AutoPortal.Order;
using AP.Business.Registration;
using AP.Business.Registration.Handlers;
using AP.Business.Registrations.Handlers;
using AP.Infrastructure;
using AP.Infrastructure.Azure;
using AP.Infrastructure.Azure.BlobStorage;
using AP.Infrastructure.Azure.EventSourcing;
using AP.Infrastructure.Azure.MessageLog;
using AP.Infrastructure.Azure.Messaging;
using AP.Infrastructure.Azure.Messaging.Handling;
using AP.Infrastructure.BlobStorage;
using AP.Infrastructure.EventSourcing;
using AP.Infrastructure.Messaging;
using AP.Infrastructure.Messaging.Handling;
using AP.Infrastructure.Serialization;
using AP.Registration.Handlers;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace AP.Server.Application
{
    partial class BookingContainer
    {
        partial void OnCreateContainer(IServiceCollection services, ITextSerializer serializer, IMetadataProvider metadata, ILoggerFactory loggerFactory)
        {
            var azureSettings = InfrastructureSettings.Read("Application\\Settings.xml");

            var busConfig = new ServiceBusConfig(azureSettings.ServiceBus);
            busConfig.Initialize();

            // blob
            var blobStorageAccount = CloudStorageAccount.Parse(azureSettings.BlobStorage.ConnectionString);

            services.AddSingleton<IBlobStorage>(new CloudBlobStorage(blobStorageAccount, azureSettings.BlobStorage.RootContainerName, loggerFactory.CreateLogger<CloudBlobStorage>()));
            var topicLogger = loggerFactory.CreateLogger<TopicSender>();
            var commandBus = new CommandBus(new TopicSender(azureSettings.ServiceBus, Topics.Commands.Path, topicLogger), metadata, serializer);
            var eventsTopicSender = new TopicSender(azureSettings.ServiceBus, Topics.Events.Path, topicLogger);
            services.AddSingleton<IMessageSender>(eventsTopicSender);
            services.AddSingleton<IMessageSender>(/*"orders", */new TopicSender(azureSettings.ServiceBus, Topics.EventsOrders.Path, topicLogger));
            services.AddSingleton<IMessageSender>(/*"seatsavailability",*/ new TopicSender(azureSettings.ServiceBus, Topics.EventsAvailability.Path, topicLogger));
            var eventBus = new EventBus(eventsTopicSender, metadata, serializer);

            var commonLogger = loggerFactory.CreateLogger<ILogger>();
            var sessionlessCommandProcessor =
                new CommandProcessor(new SubscriptionReceiver(azureSettings.ServiceBus, Topics.Commands.Path, Topics.Commands.Subscriptions.Sessionless, false, commonLogger), serializer);
            var seatsAvailabilityCommandProcessor =
                new CommandProcessor(new SessionSubscriptionReceiver(azureSettings.ServiceBus, Topics.Commands.Path, Topics.Commands.Subscriptions.Anchorsavailability, false, commonLogger), serializer);

            var synchronousCommandBus = new SynchronousCommandBusDecorator(commandBus, loggerFactory.CreateLogger<SynchronousCommandBusDecorator>());
            services.AddSingleton<ICommandBus>(synchronousCommandBus);

            services.AddSingleton<IEventBus>(eventBus);
            services.AddSingleton<IProcessor>(/*"SessionlessCommandProcessor", */sessionlessCommandProcessor);
            services.AddSingleton<IProcessor>(/*"SeatsAvailabilityCommandProcessor", */seatsAvailabilityCommandProcessor);

            RegisterRepositories(services, azureSettings, loggerFactory);

            var serviceProvider = services.BuildServiceProvider();

            RegisterEventProcessors(services, serviceProvider, busConfig, serializer);

            var commandHandlers = serviceProvider.GetServices<ICommandHandler>().ToList();
            RegisterCommandHandlers(services, commandHandlers, 
                sessionlessCommandProcessor, seatsAvailabilityCommandProcessor);

            // handle order commands inline, as they do not have competition.
            // TODO: Get exactly OrderCommandHandler
            synchronousCommandBus.Register(commandHandlers.First(s => s.GetType() == typeof(OrderCommandHandler)));

            // message log
            var messageLogAccount = CloudStorageAccount.Parse(azureSettings.MessageLog.ConnectionString);

            services.AddSingleton<IProcessor>(/*"EventLogger", */new AzureMessageLogListener(
                new AzureMessageLogWriter(messageLogAccount, azureSettings.MessageLog.TableName),
                new SubscriptionReceiver(azureSettings.ServiceBus, Topics.Events.Path, Topics.Events.Subscriptions.Log, true, commonLogger)));

            services.AddSingleton<IProcessor>(/*"OrderEventLogger", */new AzureMessageLogListener(
                new AzureMessageLogWriter(messageLogAccount, azureSettings.MessageLog.TableName),
                new SubscriptionReceiver(azureSettings.ServiceBus, Topics.EventsOrders.Path, Topics.EventsOrders.Subscriptions.LogOrders, true, commonLogger)));

            services.AddSingleton<IProcessor>(/*"SeatsAvailabilityEventLogger", */new AzureMessageLogListener(
                new AzureMessageLogWriter(messageLogAccount, azureSettings.MessageLog.TableName),
                new SubscriptionReceiver(azureSettings.ServiceBus, Topics.EventsAvailability.Path, Topics.EventsAvailability.Subscriptions.LogAvail, true, commonLogger)));

            services.AddSingleton<IProcessor>(/*"CommandLogger", */new AzureMessageLogListener(
                new AzureMessageLogWriter(messageLogAccount, azureSettings.MessageLog.TableName),
                new SubscriptionReceiver(azureSettings.ServiceBus, Topics.Commands.Path, Topics.Commands.Subscriptions.Log, true, commonLogger)));
        }

        private void RegisterRepositories(IServiceCollection services, InfrastructureSettings azureSettings, ILoggerFactory loggerFactory)
        {
            var eventSourcingAccount = CloudStorageAccount.Parse(azureSettings.EventSourcing.ConnectionString);
            var eventLogger = loggerFactory.CreateLogger<EventStore>();
            var eventStore = new EventStore(eventSourcingAccount, azureSettings.EventSourcing.OrdersTableName, eventLogger);
            var anchorsAvailabilityEventStore = new EventStore(eventSourcingAccount, azureSettings.EventSourcing.AnchorsAvailabilityTableName, eventLogger);

            services.AddSingleton<IEventStore>(eventStore);
            services.AddSingleton<IPendingEventsQueue>(eventStore);

            services.AddSingleton<IEventStore>(anchorsAvailabilityEventStore);
            services.AddSingleton<IPendingEventsQueue>(anchorsAvailabilityEventStore);

            services.AddSingleton<IEventStoreBusPublisher, EventStoreBusPublisher>(
                provider => new EventStoreBusPublisher(provider.GetService<IMessageSender>(),
                    provider.GetService<IPendingEventsQueue>(),
                    loggerFactory.CreateLogger<EventStoreBusPublisher>()));

            //TODO: fix repos for each eventStores
            services.AddSingleton<IEventSourcedRepository<Order>, AzureEventSourcedRepository<Order>>(
                provider => new AzureEventSourcedRepository<Order>(
                    provider.GetService<IEventStore>(), //"orders"
                    provider.GetService<IEventStoreBusPublisher>(), //"orders"
                    provider.GetService<ITextSerializer>(),
                    provider.GetService<IMetadataProvider>(),
                    provider.GetService<IMemoryCache>()));

            services.AddSingleton<IEventSourcedRepository<AnchorAssignments>, AzureEventSourcedRepository<AnchorAssignments>>(
                provider => new AzureEventSourcedRepository<AnchorAssignments>(
                    provider.GetService<IEventStore>(), //"orders"
                    provider.GetService<IEventStoreBusPublisher>(), //"orders"
                    provider.GetService<ITextSerializer>(),
                    provider.GetService<IMetadataProvider>(),
                    provider.GetService<IMemoryCache>()));

            services.AddSingleton<IEventSourcedRepository<AnchorsAvailability>, AzureEventSourcedRepository<AnchorsAvailability>>(
                provider => new AzureEventSourcedRepository<AnchorsAvailability>(
                    provider.GetService<IEventStore>(), //"anchorsavailability"
                    provider.GetService<IEventStoreBusPublisher>(), //"anchorsavailability"
                    provider.GetService<ITextSerializer>(),
                    provider.GetService<IMetadataProvider>(),
                    provider.GetService<IMemoryCache>()));

            services.AddSingleton<IProcessor>(
                //"OrdersEventStoreBusPublisher",
                provider => 
                    new PublisherProcessorAdapter(provider.GetService<IEventStoreBusPublisher>()/*"orders"*/, this.cancellationTokenSource.Token));
            services.AddSingleton<IProcessor>(
                //"SeatsAvailabilityEventStoreBusPublisher",
                provider =>
                    new PublisherProcessorAdapter(provider.GetService<IEventStoreBusPublisher>()/*"seatsavailability"*/, this.cancellationTokenSource.Token));
        }

        private void RegisterEventProcessors(IServiceCollection services, IServiceProvider provider, ServiceBusConfig busConfig, ITextSerializer serializer)
        {
            RegisterEventProcessor<RegistrationProcessManagerRouter>(services, provider, busConfig, Topics.Events.Subscriptions.RegistrationPMNextSteps, serializer);
            RegisterEventProcessor<PricedOrderViewModelGenerator>(services, provider, busConfig, Topics.Events.Subscriptions.PricedOrderViewModelGeneratorV3, serializer);
            RegisterEventProcessor<WorkshopViewModelGenerator>(services, provider, busConfig, Topics.Events.Subscriptions.WorkshopViewModelGenerator, serializer);

            RegisterEventProcessor<RegistrationProcessManagerRouter>(services, provider, busConfig, Topics.EventsOrders.Subscriptions.RegistrationPMOrderPlacedOrders, serializer);
            RegisterEventProcessor<RegistrationProcessManagerRouter>(services, provider, busConfig, Topics.EventsOrders.Subscriptions.RegistrationPMNextStepsOrders, serializer);
            RegisterEventProcessor<DraftOrderViewModelGenerator>(services, provider, busConfig, Topics.EventsOrders.Subscriptions.OrderViewModelGeneratorOrders, serializer);
            RegisterEventProcessor<PricedOrderViewModelGenerator>(services, provider, busConfig, Topics.EventsOrders.Subscriptions.PricedOrderViewModelOrders, serializer);
            RegisterEventProcessor<AnchorAssignmentsViewModelGenerator>(services, provider, busConfig, Topics.EventsOrders.Subscriptions.AnchorAssignmentsViewModelOrders, serializer);
            RegisterEventProcessor<AnchorAssignmentsHandler>(services, provider, busConfig, Topics.EventsOrders.Subscriptions.AnchorAssignmentsHandlerOrders, serializer);
            RegisterEventProcessor<OrderEventHandler>(services, provider, busConfig, Topics.EventsOrders.Subscriptions.OrderEventHandlerOrders, serializer);

            RegisterEventProcessor<RegistrationProcessManagerRouter>(services, provider, busConfig, Topics.EventsAvailability.Subscriptions.RegistrationPMNextStepsAvail, serializer);
            RegisterEventProcessor<WorkshopViewModelGenerator>(services, provider, busConfig, Topics.EventsAvailability.Subscriptions.ConferenceViewModelAvail, serializer);
        }

        private static void RegisterCommandHandlers(IServiceCollection services, List<ICommandHandler> commandHandlers,
            ICommandHandlerRegistry sessionlessRegistry, ICommandHandlerRegistry anchorsAvailabilityRegistry)
        {
            var anchorsAvailabilityHandler = commandHandlers.First(x => x.GetType().GetTypeInfo().IsAssignableFrom(typeof(AnchorsAvailabilityHandler)));

            anchorsAvailabilityRegistry.Register(anchorsAvailabilityHandler);
            foreach (var commandHandler in commandHandlers.Where(x => x != anchorsAvailabilityHandler))
            {
                sessionlessRegistry.Register(commandHandler);
            }
        }

        private void RegisterEventProcessor<T>(IServiceCollection services, IServiceProvider provider, 
            ServiceBusConfig busConfig, string subscriptionName, ITextSerializer serializer)
            where T : IEventHandler
        {
            services.AddSingleton<IProcessor>(busConfig.CreateEventProcessor(
                subscriptionName,
                provider.GetService<T>(),
                serializer));
        }

        // to satisfy the IProcessor requirements.
        // TODO: we should unify and probably use token-based Start only processors.
        private class PublisherProcessorAdapter : IProcessor
        {
            private IEventStoreBusPublisher publisher;
            private CancellationToken token;

            public PublisherProcessorAdapter(IEventStoreBusPublisher publisher, CancellationToken token)
            {
                this.publisher = publisher;
                this.token = token;
            }

            public void Start()
            {
                this.publisher.Start(this.token);
            }

            public void Stop()
            {
                // Do nothing. The cancelled token will stop the process anyway.
            }
        }
    }
}
