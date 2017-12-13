namespace AP.Business.Registration
{
    using Infrastructure.Messaging;
    using AP.Infrastructure.Messaging.Handling;
    using Infrastructure.Processes;
    //using Payments.Contracts.Events;
    using Registration.Commands;
    using Registration.Events;
    using AP.Business.Model.Registration.Events;
    using Microsoft.Extensions.Logging;
    using System;

    /// <summary>
    /// Routes messages (commands and events) to the <see cref="RegistrationProcessManager"/>.
    /// </summary>
    public class RegistrationProcessManagerRouter : //AmbientContext<RegistrationProcessManagerDbContext>,
        IEventHandler<OrderPlaced>,
        IEventHandler<OrderUpdated>,
        IEnvelopedEventHandler<AnchorsReserved>,
        //IEventHandler<PaymentCompleted>,
        IEventHandler<OrderConfirmed>,
        ICommandHandler<ExpireRegistrationProcess>
    {
        private readonly Func<IProcessManagerDataContext<RegistrationProcessManager>> contextFactory;
        private readonly ILoggerFactory loggerFactory;
        private readonly ILogger logger;

        public RegistrationProcessManagerRouter(Func<IProcessManagerDataContext<RegistrationProcessManager>> contextFactory, ILoggerFactory loggerFactory)
        {
            this.contextFactory = contextFactory;
            logger = loggerFactory.CreateLogger<RegistrationProcessManagerRouter>();
        }

        public void Handle(OrderPlaced @event)
        {
            using (var context = contextFactory.Invoke())
            {
                var pm = context.Find(x => x.OrderID == @event.SourceId);
                if (pm == null)
                {
                    pm = new RegistrationProcessManager();
                }

                pm.Handle(@event);
                context.Save(pm);
            }
        }

        public void Handle(OrderUpdated @event)
        {
            using (var context = contextFactory.Invoke())
            {
                var pm = context.Find(x => x.OrderID == @event.SourceId);
                if (pm != null)
                {
                    pm.Handle(@event);

                    context.Save(pm);
                }
                else
                {
                    logger.LogError("Failed to locate the registration process manager handling the order with id {0}.", @event.SourceId);
                }
            }
        }

        public void Handle(Envelope<AnchorsReserved> envelope)
        {
            using (var context = contextFactory.Invoke())
            {
                var pm = context.Find(x => x.ReservationID == envelope.Body.ReservationId);
                if (pm != null)
                {
                    pm.Handle(envelope);

                    context.Save(pm);
                }
                else
                {
                    // TODO: should Cancel seat reservation!
                    logger.LogError("Failed to locate the registration process manager handling the seat reservation with id {0}. TODO: should Cancel seat reservation!", envelope.Body.ReservationId);
                }
            }
        }

        public void Handle(OrderConfirmed @event)
        {
            using (var context = contextFactory.Invoke())
            {
                var pm = context.Find(x => x.OrderID == @event.SourceId);
                if (pm != null)
                {
                    pm.Handle(@event);

                    context.Save(pm);
                }
                else
                {
                    logger.LogInformation("Failed to locate the registration process manager to complete with id {0}.", @event.SourceId);
                }
            }
        }
        /*
        public void Handle(PaymentCompleted @event)
        {
            using (var context = this.contextFactory.Invoke())
            {
                // TODO: should not skip the completed processes and try to re-acquire the reservation,
                // and if not possible due to not enough seats, move them to a "manual intervention" state.
                // This was not implemented but would be very important.
                var pm = context.Find(x => x.OrderId == @event.PaymentSourceId);
                if (pm != null)
                {
                    pm.Handle(@event);

                    context.Save(pm);
                }
                else
                {
                    Trace.TraceError("Failed to locate the registration process manager handling the paid order with id {0}.", @event.PaymentSourceId);
                }
            }
        }
        */
        public void Handle(ExpireRegistrationProcess command)
        {
            using (var context = contextFactory.Invoke())
            {
                var pm = context.Find(x => x.ID == command.ProcessId);
                if (pm != null)
                {
                    pm.Handle(command);

                    context.Save(pm);
                }
            }
        }
    }
}
