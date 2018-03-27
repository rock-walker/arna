namespace AP.Business.Registration.Handlers
{
    using AP.Business.Model.Registration.Events;
    using AP.Business.Registration.Commands;
    using AP.Infrastructure.EventSourcing;
    using AP.Infrastructure.Messaging.Handling;

    public class AnchorAssignmentsHandler :
        IEventHandler<OrderConfirmed>,
        ICommandHandler<UnassignAnchor>,
        ICommandHandler<AssignAnchor>
    {
        private readonly IEventSourcedRepository<Order> ordersRepo;
        private readonly IEventSourcedRepository<AnchorAssignments> assignmentsRepo;

        public AnchorAssignmentsHandler(IEventSourcedRepository<Order> ordersRepo, IEventSourcedRepository<AnchorAssignments> assignmentsRepo)
        {
            this.ordersRepo = ordersRepo;
            this.assignmentsRepo = assignmentsRepo;
        }

        public void Handle(OrderConfirmed @event)
        {
            var order = this.ordersRepo.Get(@event.SourceId);
            var assignments = order.CreateAnchorAssignments();
            assignmentsRepo.Save(assignments, null);
        }

        public void Handle(AssignAnchor command)
        {
            var assignments = this.assignmentsRepo.Get(command.SeatAssignmentsId);
            assignments.AssignAnchor(command.Position, command.Attendee);
            assignmentsRepo.Save(assignments, command.Id.ToString());
        }

        public void Handle(UnassignAnchor command)
        {
            var assignments = this.assignmentsRepo.Get(command.SeatAssignmentsId);
            assignments.Unassign(command.Position);
            assignmentsRepo.Save(assignments, command.Id.ToString());
        }
    }
}
