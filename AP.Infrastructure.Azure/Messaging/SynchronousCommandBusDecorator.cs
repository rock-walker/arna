namespace AP.Infrastructure.Azure.Messaging
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using AP.Infrastructure.Azure.Messaging.Handling;
    using AP.Infrastructure.Messaging;
    using AP.Infrastructure.Messaging.Handling;
    using Microsoft.Extensions.Logging;

    public class SynchronousCommandBusDecorator : ICommandBus, ICommandHandlerRegistry
    {
        private readonly ICommandBus commandBus;
        private readonly CommandDispatcher commandDispatcher;
        private readonly ILogger<SynchronousCommandBusDecorator> logger;

        public SynchronousCommandBusDecorator(ICommandBus commandBus, ILogger<SynchronousCommandBusDecorator> logger)
        {
            this.commandBus = commandBus;
            this.commandDispatcher = new CommandDispatcher();
            this.logger = logger;
        }

        public void Register(ICommandHandler commandHandler)
        {
            this.commandDispatcher.Register(commandHandler);
        }

        public void Send(Envelope<ICommand> command)
        {
            if (!this.DoSend(command))
            {
                // Trace.TraceInformation("Command with id {0} was not handled locally. Sending it through the bus.", command.Body.Id);
                this.commandBus.Send(command);
            }
        }

        public void Send(IEnumerable<Envelope<ICommand>> commands)
        {
            var pending = commands.ToList();

            while (pending.Count > 0)
            {
                if (this.DoSend(pending[0]))
                {
                    pending.RemoveAt(0);
                }
                else
                {
                    break;
                }
            }

            if (pending.Count > 0)
            {
                // Trace.TraceInformation("Command with id {0} was not handled locally. Sending it and all remaining commands through the bus.", pending[0].Body.Id);
                this.commandBus.Send(pending);
            }
        }

        private bool DoSend(Envelope<ICommand> command)
        {
            bool handled = false;

            try
            {
                var traceIdentifier = string.Format(CultureInfo.CurrentCulture, " (local handling of command with id {0})", command.Body.Id);
                handled = this.commandDispatcher.ProcessMessage(traceIdentifier, command.Body, command.MessageId, command.CorrelationId);

                // TODO try to log the command
            }
            catch (Exception e)
            {
                logger.LogWarning("Exception handling command with id {0} synchronously: {1}. Command will be sent through the bus.", command.Body.Id, e.Message);
            }

            return handled;
        }
    }
}
