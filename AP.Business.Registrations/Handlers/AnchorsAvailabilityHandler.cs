namespace AP.Registration.Handlers
{
    using AP.Business.Registration;
    using AP.Business.Registration.Commands;
    using Infrastructure.EventSourcing;
    using Infrastructure.Messaging.Handling;

    /// <summary>
    /// Handles commands issued to the anchors availability aggregate.
    /// </summary>
    public class AnchorsAvailabilityHandler :
        ICommandHandler<MakeAnchorReservation>,
        ICommandHandler<CancelAnchorReservation>,
        ICommandHandler<CommitAnchorReservation>,
        ICommandHandler<AddAnchors>,
        ICommandHandler<RemoveAnchors>
    {
        private readonly IEventSourcedRepository<AnchorsAvailability> repository;

        public AnchorsAvailabilityHandler(IEventSourcedRepository<AnchorsAvailability> repository)
        {
            this.repository = repository;
        }

        public void Handle(MakeAnchorReservation command)
        {
            var availability = this.repository.Get(command.WorkshopID);
            availability.MakeReservation(command.ReservationId, command.Anchors);
            this.repository.Save(availability, command.Id.ToString());
        }

        public void Handle(CancelAnchorReservation command)
        {
            var availability = this.repository.Get(command.WorkshopID);
            availability.CancelReservation(command.ReservationId);
            this.repository.Save(availability, command.Id.ToString());
        }

        public void Handle(CommitAnchorReservation command)
        {
            var availability = this.repository.Get(command.WorkshopID);
            availability.CommitReservation(command.ReservationId);
            this.repository.Save(availability, command.Id.ToString());
        }

        public void Handle(AddAnchors command)
        {
            var availability = this.repository.Find(command.WorkshopID);
            if (availability == null)
                availability = new AnchorsAvailability(command.WorkshopID);

            availability.AddAnchors(command.AnchorType, command.Quantity);
            repository.Save(availability, command.Id.ToString());
        }

        public void Handle(RemoveAnchors command)
        {
            var availability = this.repository.Find(command.WorkshopID);
            if (availability == null)
                availability = new AnchorsAvailability(command.WorkshopID);

            availability.RemoveAnchors(command.AnchorType, command.Quantity);
            this.repository.Save(availability, command.Id.ToString());
        }
    }
}
