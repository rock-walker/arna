﻿namespace AP.Infrastructure.Azure.Messaging
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Infrastructure.Messaging;
    using Infrastructure.Serialization;
    using Microsoft.Azure.ServiceBus;

    /// <summary>
    /// A command bus that sends serialized object payloads through a <see cref="IMessageSender"/>.
    /// </summary>
    public class CommandBus : ICommandBus
    {
        private readonly IMessageSender sender;
        private readonly IMetadataProvider metadataProvider;
        private readonly ITextSerializer serializer;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandBus"/> class.
        /// </summary>
        public CommandBus(IMessageSender sender, IMetadataProvider metadataProvider, ITextSerializer serializer)
        {
            this.sender = sender;
            this.metadataProvider = metadataProvider;
            this.serializer = serializer;
        }

        /// <summary>
        /// Sends the specified command.
        /// </summary>
        public void Send(Envelope<ICommand> command)
        {
            this.sender.Send(() => BuildMessage(command));
        }

        public void Send(IEnumerable<Envelope<ICommand>> commands)
        {
            foreach (var command in commands)
            {
                this.Send(command);
            }
        }

        private Message BuildMessage(Envelope<ICommand> command)
        {
            var stream = new MemoryStream();
            try
            {
                var writer = new StreamWriter(stream);
                serializer.Serialize(writer, command.Body);
                stream.Position = 0;

                var message = new Message(stream.ToArray());

                if (!string.IsNullOrWhiteSpace(command.MessageId))
                {
                    message.MessageId = command.MessageId;
                }
                else if (!default(Guid).Equals(command.Body.Id))
                {
                    message.MessageId = command.Body.Id.ToString();
                }

                if (!string.IsNullOrWhiteSpace(command.CorrelationId))
                {
                    message.CorrelationId = command.CorrelationId;
                }

                var sessionProvider = command.Body as IMessageSessionProvider;
                if (sessionProvider != null)
                {
                    message.SessionId = sessionProvider.SessionId;
                }

                var metadata = this.metadataProvider.GetMetadata(command.Body);
                if (metadata != null)
                {
                    foreach (var pair in metadata)
                    {
                        message.UserProperties[pair.Key] = pair.Value;
                    }
                }

                if (command.Delay > TimeSpan.Zero)
                {
                    message.ScheduledEnqueueTimeUtc = DateTime.UtcNow.Add(command.Delay);
                }

                if (command.TimeToLive > TimeSpan.Zero)
                {
                    message.TimeToLive = command.TimeToLive;
                }

                return message;
            }
            catch
            {
                stream.Dispose();
                throw;
            }
        }
    }
}
