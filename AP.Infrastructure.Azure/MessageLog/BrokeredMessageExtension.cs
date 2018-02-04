namespace AP.Infrastructure.Azure.MessageLog
{
    using System.Collections.Generic;
    using Microsoft.Azure.ServiceBus;
    using System.Text;

    public static class MessageExtension
    {
        public static MessageLogEntity ToMessageLogEntity(this Message message)
        {
            var body = message.Body;
            var payload = Encoding.UTF8.GetString(body);

            return new MessageLogEntity
            {
                PartitionKey = message.SystemProperties.EnqueuedTimeUtc.ToString("yyyMM"),
                RowKey = message.SystemProperties.EnqueuedTimeUtc.Ticks.ToString("D20") + "_" + message.MessageId,
                MessageId = message.MessageId,
                CorrelationId = message.CorrelationId,
                SourceId = message.UserProperties.TryGetValue(StandardMetadata.SourceId) as string,
                Kind = message.UserProperties.TryGetValue(StandardMetadata.Kind) as string,
                AssemblyName = message.UserProperties.TryGetValue(StandardMetadata.AssemblyName) as string,
                FullName = message.UserProperties.TryGetValue(StandardMetadata.FullName) as string,
                Namespace = message.UserProperties.TryGetValue(StandardMetadata.Namespace) as string,
                TypeName = message.UserProperties.TryGetValue(StandardMetadata.TypeName) as string,
                SourceType = message.UserProperties.TryGetValue(StandardMetadata.SourceType) as string,
                CreationDate = message.SystemProperties.EnqueuedTimeUtc.ToString("o"),
                Payload = payload,
            };
        }
    }
}
