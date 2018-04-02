namespace AP.Infrastructure.Azure.Messaging.Handling
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Infrastructure.Messaging;
    using Infrastructure.Messaging.Handling;
    using System.Reflection;

    public class CommandDispatcher
    {
        private Dictionary<Type, ICommandHandler> handlers = new Dictionary<Type, ICommandHandler>();

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandDispatcher"/> class.
        /// </summary>
        public CommandDispatcher()
        {
        }

        /// <summary>
        /// Registers the specified command handler.
        /// </summary>
        public void Register(ICommandHandler commandHandler)
        {
            var genericHandler = typeof(ICommandHandler<>);
            var supportedCommandTypes = commandHandler.GetType().GetTypeInfo()
                .GetInterfaces()
                .Where(iface => iface.GetTypeInfo().IsGenericType && iface.GetGenericTypeDefinition() == genericHandler)
                .Select(iface => iface.GetGenericArguments()[0])
                .ToList();

            if (handlers.Keys.Any(registeredType => supportedCommandTypes.Contains(registeredType)))
                throw new ArgumentException("The command handled by the received handler already has a registered handler.");

            // Register this handler for each of he handled types.
            foreach (var commandType in supportedCommandTypes)
            {
                this.handlers.Add(commandType, commandHandler);
            }
        }

        /// <summary>
        /// Processes the message by calling the registered handler.
        /// </summary>
        public bool ProcessMessage(string traceIdentifier, ICommand payload, string messageId, string correlationId)
        {
            var commandType = payload.GetType();
            ICommandHandler handler = null;

            if (this.handlers.TryGetValue(commandType, out handler))
            {
                // Trace.WriteLine(string.Format(CultureInfo.InvariantCulture, "Command{0} handled by {1}.", traceIdentifier, handler.GetType().FullName));
                ((dynamic)handler).Handle((dynamic)payload);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
