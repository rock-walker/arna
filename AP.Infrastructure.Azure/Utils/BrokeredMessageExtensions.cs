namespace AP.Infrastructure.Azure.Utils
{
    using System;
    using System.Diagnostics;
    using Microsoft.Azure.ServiceBus;
    using Polly;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using Microsoft.Azure.ServiceBus.Core;

    public static class BrokeredMessageExtensions
    {
        public static async Task SafeCompleteAsync(this Message message, string subscription, IReceiverClient session, Action<bool> callback, ILogger logger, Stopwatch roundtripStopwatch)
        {
            await SafeMessagingActionAsync(
                session.CompleteAsync(message.SystemProperties.LockToken),
                message,
                callback,
                "An error occurred while completing message {0} in subscription {1} with processing time {3} (scheduling {4} request {5} roundtrip {6}). Error message: {2}",
                message.MessageId,
                subscription,
                logger,
                roundtripStopwatch);
        }

        public static async Task SafeAbandonAsync(this Message message, string subscription, IReceiverClient session, Action<bool> callback, ILogger logger, Stopwatch roundtripStopwatch)
        {
            await SafeMessagingActionAsync(
                session.AbandonAsync(message.SystemProperties.LockToken),
                message,
                callback,
                "An error occurred while abandoning message {0} in subscription {1} with processing time {3} (scheduling {4} request {5} roundtrip {6}). Error message: {2}",
                message.MessageId,
                subscription,
                logger,
                roundtripStopwatch);
        }

        public static async Task SafeDeadLetterAsync(this Message message, string subscription, IReceiverClient session, string reason, string description, Action<bool> callback, ILogger logger, Stopwatch roundtripStopwatch)
        {
            await SafeMessagingActionAsync(
                session.DeadLetterAsync(message.SystemProperties.LockToken),
                message,
                callback,
                "An error occurred while dead-lettering message {0} in subscription {1} with processing time {3} (scheduling {4} request {5} roundtrip {6}). Error message: {2}",
                message.MessageId,
                subscription,
                logger,
                roundtripStopwatch);
        }

        internal static async Task SafeMessagingActionAsync(Task task, Message message, Action<bool> callback, string actionErrorDescription, 
            string messageId, string subscription, ILogger logger, /*long processingElapsedMilliseconds, long schedulingElapsedMilliseconds, */Stopwatch roundtripStopwatch)
        {
            Polly.Retry.RetryPolicy retryPolicy = Policy.Handle<Exception>()
                .WaitAndRetryAsync(3, (retry) => TimeSpan.FromSeconds(2),
                    (ex, ts, attempt, context) =>
                    {
                        logger.LogWarning($"An error occurred in attempt number {attempt} to release message {message.MessageId}" +
                            $" in subscription \"{subscription}\": {ex.GetType().Name + " - " + ex.Message}");
                    }
                );

            long messagingActionStart = 0;

            await retryPolicy.ExecuteAsync(
                async () => {
                    try
                    {
                        messagingActionStart = roundtripStopwatch.ElapsedMilliseconds;
                        await task;

                        roundtripStopwatch.Stop();
                        callback(true);
                    }
                    catch (Exception e)
                    {
                        roundtripStopwatch.Stop();

                        if (e is MessageLockLostException || /*ex is MessagingException ||*/ e is TimeoutException)
                        {
                            logger.LogWarning(actionErrorDescription, messageId, subscription, e.GetType().Name + " - " + e.Message, 
                                messagingActionStart, roundtripStopwatch.ElapsedMilliseconds);
                        }
                        else
                        {
                            logger.LogError($"Unexpected error releasing message in subscription \"{subscription}\": {e.GetType().Name + " - " + e.Message}");
                        }

                        callback(false);
                    }
                });
        }
    }
}
