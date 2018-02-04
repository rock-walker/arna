namespace AP.Infrastructure.Azure.Messaging
{
    using System;
    using System.Globalization;
    using System.Linq;
    using AP.Infrastructure.Azure.Messaging.Handling;
    using AP.Infrastructure.Messaging.Handling;
    using AP.Infrastructure.Serialization;
    using Microsoft.Azure.ServiceBus;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using Polly;

    public class ServiceBusConfig
    {
        private const string RuleName = "Custom";
        private bool initialized;
        private ServiceBusSettings settings;
        private readonly ILogger logger;

        public ServiceBusConfig(ServiceBusSettings settings)
        {
            this.settings = settings;
            this.logger = new LoggerFactory().CreateLogger<ServiceBusConfig>();
        }

        public void Initialize()
        {
            Func<int, TimeSpan> retryStrategy = (r) => TimeSpan.FromMilliseconds(100);//, TimeSpan.FromSeconds(1));
            var retryPolicy = Policy.Handle<Exception>().WaitAndRetry(3, retryStrategy);
            //var serviceUri = ServiceBusEnvironment.CreateServiceUri(settings.ServiceUriScheme, settings.ServiceNamespace, settings.ServicePath);
            //var namespaceManager = new NamespaceManager(serviceUri, tokenProvider);
            var namespaceManager = new ServiceBusConnectionStringBuilder("");
            this.settings.Topics.AsParallel().ForAll(topic =>
            {
                //retryPolicy.Execute(() => CreateTopicIfNotExists(namespaceManager, topic));
                topic.Subscriptions.AsParallel().ForAll(subscription =>
                {
                    //retryPolicy.Execute(() => CreateSubscriptionIfNotExists(namespaceManager, topic, subscription));
                    //retryPolicy.Execute(() => UpdateRules(namespaceManager, topic, subscription));
                });
            });

            // Execute migration support actions only after all the previous ones have been completed.
            foreach (var topic in this.settings.Topics)
            {
                foreach (var action in topic.MigrationSupport)
                {
                    //retryPolicy.Execute(() => UpdateSubscriptionIfExists(namespaceManager, topic, action));
                }
            }

            this.initialized = true;
        }

        // Can't really infer the topic from the subscription, since subscriptions of the same 
        // name can exist across different topics (i.e. "all" currently)
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Instrumentation disposed by receiver")]
        public EventProcessor CreateEventProcessor(string subscription, IEventHandler handler, ITextSerializer serializer)
        {
            if (!this.initialized)
                throw new InvalidOperationException("Service bus configuration has not been initialized.");

            TopicSettings topicSettings = null;
            SubscriptionSettings subscriptionSettings = null;

            foreach (var settings in this.settings.Topics.Where(t => t.IsEventBus))
            {
                subscriptionSettings = settings.Subscriptions.Find(s => s.Name == subscription);
                if (subscriptionSettings != null)
                {
                    topicSettings = settings;
                    break;
                }
            }

            if (subscriptionSettings == null)
            {
                throw new ArgumentOutOfRangeException(
                    string.Format(
                        CultureInfo.CurrentCulture,
                        "Subscription '{0}' has not been registered for an event bus topic in the service bus configuration.",
                        subscription));
            }

            IMessageReceiver receiver;

            if (subscriptionSettings.RequiresSession)
            {
                //var instrumentation = new SessionSubscriptionReceiverInstrumentation(subscription, instrumentationEnabled);
                try
                {
                    receiver = (IMessageReceiver)new SessionSubscriptionReceiver(this.settings, topicSettings.Path, subscription, true, logger);//instrumentation);
                }
                catch
                {
                    //instrumentation.Dispose();
                    throw;
                }
            }
            else
            {
                //var instrumentation = new SubscriptionReceiverInstrumentation(subscription, instrumentationEnabled);
                try
                {
                    receiver = (IMessageReceiver)new SubscriptionReceiver(this.settings, topicSettings.Path, subscription, true, logger);//instrumentation);
                }
                catch
                {
                //    instrumentation.Dispose();
                    throw;
                }
            }

            EventProcessor processor;
            try
            {
                processor = new EventProcessor(receiver, serializer);
            }
            catch
            {
                using (receiver as IDisposable) { }
                throw;
            }

            try
            {
                processor.Register(handler);

                return processor;
            }
            catch
            {
                processor.Dispose();
                throw;
            }
        }

        private void CreateTopicIfNotExists(string connection, TopicSettings topic)
        {
            var topicClient = new TopicClient(connection, topic.Path);
            TopicSettings settings = new TopicSettings
            {
                DuplicateDetectionHistoryTimeWindow = topic.DuplicateDetectionHistoryTimeWindow,
                Path = topic.Path
            };

                /*
            var topicDescription = 
                new TopicDescription(topic.Path)
                {
                    RequiresDuplicateDetection = true,
                    DuplicateDetectionHistoryTimeWindow = topic.DuplicateDetectionHistoryTimeWindow,
                };
                */
            try
            {
                //namespaceManager.CreateTopic(topicDescription);

            }
            //catch (MessagingEntityAlreadyExistsException) { }
            catch (ServiceBusException) { }
        }
        /*
        private void CreateSubscriptionIfNotExists(NamespaceManager namespaceManager, TopicSettings topic, SubscriptionSettings subscription)
        {
            var subscriptionDescription =
                new SubscriptionDescription(topic.Path, subscription.Name)
                {
                    RequiresSession = subscription.RequiresSession,
                    LockDuration = TimeSpan.FromSeconds(150),
                };

            try
            {
                namespaceManager.CreateSubscription(subscriptionDescription);
            }
            //catch (MessagingEntityAlreadyExistsException) { }
            catch (ServiceBusException) { }
        }
        
        private static void UpdateSubscriptionIfExists(NamespaceManager namespaceManager, TopicSettings topic, UpdateSubscriptionIfExists action)
        {
            if (string.IsNullOrWhiteSpace(action.Name)) throw new ArgumentException("action");
            if (string.IsNullOrWhiteSpace(action.SqlFilter)) throw new ArgumentException("action");

            UpdateSqlFilter(namespaceManager, action.SqlFilter, action.Name, topic.Path);
        }

        private static void UpdateRules(NamespaceManager namespaceManager, TopicSettings topic, SubscriptionSettings subscription)
        {
            string sqlExpression = null;
            if (!string.IsNullOrWhiteSpace(subscription.SqlFilter))
            {
                sqlExpression = subscription.SqlFilter;
            }

            UpdateSqlFilter(namespaceManager, sqlExpression, subscription.Name, topic.Path);
        }
/*
        private static async void UpdateSqlFilter(NamespaceManager namespaceManager, string sqlExpression, string subscriptionName, string topicPath)
        {
            bool needsReset = false;
            List<RuleDescription> existingRules;
            try
            {
                existingRules = namespaceManager.GetRules(topicPath, subscriptionName).ToList();
            }
            catch (MessagingEntityNotFoundException)
            {
                // the subscription does not exist, no need to update rules.
                return;
            }
            if (existingRules.Count != 1)
            {
                needsReset = true;
            }
            else
            {
                var existingRule = existingRules.First();
                if (sqlExpression != null && existingRule.Name == RuleDescription.DefaultRuleName)
                {
                    needsReset = true;
                }
                else if (sqlExpression == null && existingRule.Name != RuleDescription.DefaultRuleName)
                {
                    needsReset = true;
                }
                else if (sqlExpression != null && existingRule.Name != RuleName)
                {
                    needsReset = true;
                }
                else if (sqlExpression != null && existingRule.Name == RuleName)
                {
                    var filter = existingRule.Filter as SqlFilter;
                    if (filter == null || filter.SqlExpression != sqlExpression)
                    {
                        needsReset = true;
                    }
                }
            }

            if (needsReset)
            {
                SubscriptionClient client = null;
                try
                {
                    client = new SubscriptionClient(namespaceManager.Address, topicPath, subscriptionName);

                    // first add the default rule, so no new messages are lost while we are updating the subscription
                    await TryAddRule(client, new RuleDescription(RuleDescription.DefaultRuleName, new TrueFilter()));

                    // then delete every rule but the Default one
                    foreach (var existing in existingRules.Where(x => x.Name != RuleDescription.DefaultRuleName))
                    {
                        await TryRemoveRule(client, existing.Name);
                    }

                    if (sqlExpression != null)
                    {
                        // Add the desired rule.
                        await TryAddRule(client, new RuleDescription(RuleName, new SqlFilter(sqlExpression)));

                        // once the desired rule was added, delete the default rule.
                        await TryRemoveRule(client, RuleDescription.DefaultRuleName);
                    }
                }
                finally
                {
                    if (client != null)
                    {
                        await client.CloseAsync();
                        //client.Close();
                    }
                }
            }
        }
*/
        private static async Task TryAddRule(SubscriptionClient client, RuleDescription rule)
        {
            // try / catch is because there could be other processes initializing at the same time.
            try
            {
                await client.AddRuleAsync(rule);
            }
            //TODO: pay attention to this type of exception
            //catch (MessagingEntityAlreadyExistsException) { }
            catch (MessagingEntityDisabledException) { }
        }

        private static async Task TryRemoveRule(SubscriptionClient client, string ruleName)
        {
            // try / catch is because there could be other processes initializing at the same time.
            try
            {
                await client.RemoveRuleAsync(ruleName);
            }
            catch (MessagingEntityNotFoundException) { }
        }
    }
}
