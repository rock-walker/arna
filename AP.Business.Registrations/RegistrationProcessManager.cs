namespace AP.Business.Registration
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using AP.Infrastructure.Messaging;
    using AP.Infrastructure.Processes;
    using AP.Infrastructure.Utils;
    using AP.Business.Registration.Commands;
    using AP.Business.Registration.Events;
    using System.ComponentModel.DataAnnotations.Schema;
    using AP.Business.Model.Registration.Events;

    /// <summary>
    /// Represents a Process Manager that is in charge of communicating between the different distributed components
    /// when registering to a conference, reserving the seats, expiring the reservation in case the order is not
    /// completed in time, etc.
    /// </summary>
    /// <remarks>
    /// <para>For more information on the domain, see <see cref="http://go.microsoft.com/fwlink/p/?LinkID=258553">Journey chapter 3</see>.</para>
    /// <para>For more information on Process Managers, see <see cref="http://go.microsoft.com/fwlink/p/?LinkID=258564">Reference 6</see>.</para>
    /// <para>For more information on the optimizations and hardening we did to this class, and for more potential performance and scalability optimizations, 
    /// see <see cref="http://go.microsoft.com/fwlink/p/?LinkID=258557"> Journey chapter 7</see>.</para>
    /// <para>There are a few things that we learnt along the way regarding Process Managers, which we might do differently with the new insights that we
    /// now have. See <see cref="http://go.microsoft.com/fwlink/p/?LinkID=258558"> Journey lessons learnt</see> for more information.</para>
    /// </remarks>
    public class RegistrationProcessManager : IProcessManager
    {
        private static readonly TimeSpan BufferTimeBeforeReleasingAnchorsAfterExpiration = TimeSpan.FromMinutes(14);

        public enum ProcessState
        {
            NotStarted = 0,
            AwaitingReservationConfirmation = 1,
            ReservationConfirmationReceived = 2,
            PaymentConfirmationReceived = 3,
        }

        private readonly List<Envelope<ICommand>> commands = new List<Envelope<ICommand>>();

        public RegistrationProcessManager()
        {
            ID = GuidUtil.NewSequentialId();
        }

        public Guid ID { get; private set; }
        public bool Completed { get; private set; }
        public Guid WorkshopID { get; set; }
        public Guid OrderID { get; internal set; }
        public Guid ReservationID { get; internal set; }
        public Guid AnchorReservationCommandID { get; internal set; }

        // feels awkward and possibly disrupting to store these properties here. Would it be better if instead of using 
        // current state values, we use event sourcing?
        public DateTime? ReservationAutoExpiration { get; internal set; }
        public Guid ExpirationCommandId { get; set; }

        public int StateValue { get; private set; }
        [NotMapped]
        public ProcessState State
        {
            get { return (ProcessState)this.StateValue; }
            internal set { this.StateValue = (int)value; }
        }

        [ConcurrencyCheck]
        [Timestamp]
        public byte[] TimeStamp { get; private set; }

        [NotMapped]
        public IEnumerable<Envelope<ICommand>> Commands
        {
            get { return this.commands; }
        }

        public void Handle(OrderPlaced message)
        {
            if (this.State == ProcessState.NotStarted)
            {
                this.WorkshopID = message.WorkshopId;
                this.OrderID = message.SourceId;
                // Use the order id as an opaque reservation id for the anchor reservation. 
                // It could be anything else, as long as it is deterministic from the OrderPlaced event.
                this.ReservationID = message.SourceId;
                this.ReservationAutoExpiration = message.ReservationAutoExpiration;
                var expirationWindow = message.ReservationAutoExpiration.Subtract(DateTime.UtcNow);

                if (expirationWindow > TimeSpan.Zero)
                {
                    this.State = ProcessState.AwaitingReservationConfirmation;
                    var anchorReservationCommand =
                        new MakeAnchorReservation
                        {
                            WorkshopID = this.WorkshopID,
                            ReservationId = this.ReservationID,
                            Anchors = message.Anchors.ToList()
                        };
                    this.AnchorReservationCommandID = anchorReservationCommand.Id;

                    this.AddCommand(new Envelope<ICommand>(anchorReservationCommand)
                    {
                        TimeToLive = expirationWindow.Add(TimeSpan.FromMinutes(1)),
                    });

                    var expirationCommand = new ExpireRegistrationProcess { ProcessId = this.ID };
                    this.ExpirationCommandId = expirationCommand.Id;
                    this.AddCommand(new Envelope<ICommand>(expirationCommand)
                    {
                        Delay = expirationWindow.Add(BufferTimeBeforeReleasingAnchorsAfterExpiration),
                    });
                }
                else
                {
                    AddCommand(new RejectOrder { OrderId = this.OrderID });
                    Completed = true;
                }
            }
            else
            {
                if (message.WorkshopId != this.WorkshopID)
                {
                    // throw only if not reprocessing
                    throw new InvalidOperationException();
                }
            }
        }

        public void Handle(OrderUpdated message)
        {
            if (this.State == ProcessState.AwaitingReservationConfirmation
                || this.State == ProcessState.ReservationConfirmationReceived)
            {
                this.State = ProcessState.AwaitingReservationConfirmation;

                var anchorReservationCommand =
                    new MakeAnchorReservation
                    {
                        WorkshopID = this.WorkshopID,
                        ReservationId = this.ReservationID,
                        Anchors = message.Anchors.ToList()
                    };
                this.AnchorReservationCommandID = anchorReservationCommand.Id;
                this.AddCommand(anchorReservationCommand);
            }
            else
            {
                throw new InvalidOperationException("The order cannot be updated at this stage.");
            }
        }

        public void Handle(Envelope<AnchorsReserved> envelope)
        {
            if (this.State == ProcessState.AwaitingReservationConfirmation)
            {
                if (envelope.CorrelationId != null)
                {
                    if (string.CompareOrdinal(this.AnchorReservationCommandID.ToString(), envelope.CorrelationId) != 0)
                    {
                        // skip this event
                        //TODO: use logger in .net core
                        //Trace.TraceWarning("Anchor reservation response for reservation id {0} does not match the expected correlation id.", envelope.Body.ReservationId);
                        return;
                    }
                }

                this.State = ProcessState.ReservationConfirmationReceived;

                this.AddCommand(new MarkAnchorsAsReserved
                {
                    OrderId = this.OrderID,
                    Anchors = envelope.Body.ReservationDetails.ToList(),
                    Expiration = this.ReservationAutoExpiration.Value,
                });
            }
            else if (string.CompareOrdinal(this.AnchorReservationCommandID.ToString(), envelope.CorrelationId) == 0)
            {
                //TODO: use logger in .net core
                //Trace.TraceInformation("Anchor reservation response for request {1} for reservation id {0} was already handled. Skipping event.", envelope.Body.ReservationId, envelope.CorrelationId);
            }
            else
            {
                throw new InvalidOperationException("Cannot handle anchor reservation at this stage.");
            }
        }
        /*
        public void Handle(PaymentCompleted @event)
        {
            if (this.State == ProcessState.ReservationConfirmationReceived)
            {
                this.State = ProcessState.PaymentConfirmationReceived;
                this.AddCommand(new ConfirmOrder { OrderId = this.OrderId });
            }
            else
            {
                throw new InvalidOperationException("Cannot handle payment confirmation at this stage.");
            }
        }
        */
        public void Handle(OrderConfirmed @event)
        {
            if (this.State == ProcessState.ReservationConfirmationReceived || this.State == ProcessState.PaymentConfirmationReceived)
            {
                this.ExpirationCommandId = Guid.Empty;
                this.Completed = true;

                this.AddCommand(new CommitAnchorReservation
                {
                    ReservationId = this.ReservationID,
                    WorkshopID = this.WorkshopID
                });
            }
            else
            {
                throw new InvalidOperationException("Cannot handle order confirmation at this stage.");
            }
        }

        public void Handle(ExpireRegistrationProcess command)
        {
            if (this.ExpirationCommandId == command.Id)
            {
                this.Completed = true;

                this.AddCommand(new RejectOrder { OrderId = this.OrderID });
                this.AddCommand(new CancelAnchorReservation
                {
                    WorkshopID = this.WorkshopID,
                    ReservationId = this.ReservationID,
                });

                // TODO cancel payment if any
            }

            // else ignore the message as it is no longer relevant (but not invalid)
        }

        private void AddCommand<T>(T command)
            where T : ICommand
        {
            this.commands.Add(Envelope.Create<ICommand>(command));
        }

        private void AddCommand(Envelope<ICommand> envelope)
        {
            this.commands.Add(envelope);
        }
    }
}
