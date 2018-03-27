namespace AP.Business.Registration.Handlers
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Text;
    using AutoMapper;
    using Infrastructure.BlobStorage;
    using Infrastructure.Messaging.Handling;
    using Infrastructure.Serialization;
    using AP.EntityModel.ReadModel;
    using AP.Business.Model.Registration.Events;
    using System.Collections.Generic;
    using AP.Business.Registration.ReadModel;
    using AP.Core.Database;
    using AP.Repository.Context;
    using EntityFramework.DbContextScope.Interfaces;

    public class AnchorAssignmentsViewModelGenerator : AmbientContext<WorkshopRegistrationDbContext>,
        IEventHandler<AnchorAssignmentsCreated>,
        IEventHandler<AnchorAssigned>,
        IEventHandler<AnchorUnassigned>,
        IEventHandler<AnchorAssignmentUpdated>
    {
        private readonly IBlobStorage storage;
        private readonly ITextSerializer serializer;
        private readonly IWorkshopDao workshopDao;
        private readonly IDbContextScopeFactory factory;

        public AnchorAssignmentsViewModelGenerator (
            IWorkshopDao workshopDao, IBlobStorage storage, ITextSerializer serializer,
            IDbContextScopeFactory factory, IAmbientDbContextLocator locator)
            : base (locator)
        {
            this.workshopDao = workshopDao;
            this.storage = storage;
            this.serializer = serializer;
            this.factory = factory;
        }
        /*
        static AnchorAssignmentsViewModelGenerator()
        {
            Mapper.Initialize(cfg => cfg.CreateMap<AnchorAssigned, OrderSeat>());
            Mapper.Initialize(cfg => cfg.CreateMap<AnchorAssignmentUpdated, OrderSeat>());
        }
        */
        public static string GetAnchorAssignmentsBlobId(Guid sourceId)
        {
            return "AnchorAssignments-" + sourceId.ToString();
        }

        public void Handle(AnchorAssignmentsCreated @event)
        {
            using (var scope = factory.CreateReadOnly())
            {
                var seatTypes = this.workshopDao.GetAnchorTypeNames(@event.Anchors.Select(x => x.SeatType))
                    .ToDictionary(x => x.ID, x => x.Name);

                var dto = new OrderAnchors(@event.SourceId, @event.OrderId, @event.Anchors.Select(i =>
                        new OrderAnchor(i.Position, seatTypes.TryGetValue(i.SeatType))));
                Save(dto);
            }
        }

        public void Handle(AnchorAssigned @event)
        {
            var dto = Find(@event.SourceId);
            var seat = dto.Anchors.First(x => x.Position == @event.Position);
            Mapper.Map(@event, seat);
            Save(dto);
        }

        public void Handle(AnchorUnassigned @event)
        {
            var dto = Find(@event.SourceId);
            var seat = dto.Anchors.First(x => x.Position == @event.Position);
            seat.Attendee.Email = seat.Attendee.FirstName = seat.Attendee.LastName = null;
            Save(dto);
        }

        public void Handle(AnchorAssignmentUpdated @event)
        {
            var dto = Find(@event.SourceId);
            var seat = dto.Anchors.First(x => x.Position == @event.Position);
            Mapper.Map(@event, seat);
            Save(dto);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times", Justification = "By design")]
        private OrderAnchors Find(Guid id)
        {
            //TODO: move to async version of this method
            var dto = this.storage.Find(GetAnchorAssignmentsBlobId(id)).Result;
            if (dto == null)
                return null;

            using (var stream = new MemoryStream(dto))
            using (var reader = new StreamReader(stream))
            {
                return (OrderAnchors)this.serializer.Deserialize(reader);
            }
        }

        private void Save(OrderAnchors dto)
        {
            using (var writer = new StringWriter())
            {
                this.serializer.Serialize(writer, dto);
                this.storage.Save(GetAnchorAssignmentsBlobId(dto.AssignmentsId), "text/plain", Encoding.UTF8.GetBytes(writer.ToString()));
            }
        }
    }
}
