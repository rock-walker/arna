namespace AP.Business.Registration.Handlers
{
    using System.Linq;
    using AP.Infrastructure.Messaging.Handling;
    using AP.Business.Registration.Commands;
    using AP.Infrastructure.EventSourcing;

    // Note: ConfirmOrderPayment was renamed to this from V1. Make sure there are no commands pending for processing when this is deployed,
    // otherwise the ConfirmOrderPayment commands will not be processed.
    public class OrderCommandHandler :
        ICommandHandler<RegisterToWorkshop>,
        ICommandHandler<MarkAnchorsAsReserved>,
        ICommandHandler<RejectOrder>,
        ICommandHandler<AssignRegistrantDetails>,
        ICommandHandler<ConfirmOrder>
    {
        private readonly IEventSourcedRepository<Order> repository;
        private readonly IPricingService pricingService;

        public OrderCommandHandler(IEventSourcedRepository<Order> repository, IPricingService pricingService)
        {
            this.repository = repository;
            this.pricingService = pricingService;
        }
        
        public void Handle(RegisterToWorkshop command)
        {
            var items = command.Anchors.Select(t => new OrderItem(t.AnchorType, t.Quantity)).ToList();
            var order = repository.Find(command.OrderId);
            if (order == null)
            {
                order = new Order(command.OrderId, command.WorkshopId, items, pricingService);
            }
            else
            {
                order.UpdateAnchors(items, pricingService);
            }

            repository.Save(order, command.Id.ToString());
        }

        public void Handle(MarkAnchorsAsReserved command)
        {
            var order = repository.Get(command.OrderId);
            order.MarkAsReserved(this.pricingService, command.Expiration, command.Anchors);
            repository.Save(order, command.Id.ToString());
        }

        public void Handle(RejectOrder command)
        {
            var order = repository.Find(command.OrderId);
            // Explicitly idempotent. 
            if (order != null)
            {
                order.Expire();
                repository.Save(order, command.Id.ToString());
            }
        }

        public void Handle(AssignRegistrantDetails command)
        {
            var order = repository.Get(command.OrderId);
            order.AssignRegistrant(command.LastName, command.Email);
            repository.Save(order, command.Id.ToString());
        }

        public void Handle(ConfirmOrder command)
        {
            var order = repository.Get(command.OrderId);
            order.Confirm();
            repository.Save(order, command.Id.ToString());
        }
    }
}
