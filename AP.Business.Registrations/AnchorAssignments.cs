namespace AP.Business.Registration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using AutoMapper;
    using AP.Infrastructure.Utils;
    using AP.Business.Model.Registration;
    using AP.Business.Model.Registration.Events;
    using AP.Infrastructure.EventSourcing;

    /// <summary>
    /// Entity used to represent anchors asignments.
    /// </summary>
    /// <remarks>
    /// In our current business logic, 1 anchors assignments instance corresponds to 1 <see cref="Order"/> instance. 
    /// This does not need to be the case in the future.
    /// <para>For more information on the domain, see <see cref="http://go.microsoft.com/fwlink/p/?LinkID=258553">Journey chapter 3</see>.</para>
    /// </remarks>
    public class AnchorAssignments : EventSourced
    {
        public class AnchorAssignment
        {
            public AnchorAssignment()
            {
                this.Attendee = new PersonalInfo();
            }

            public int Position { get; set; }

            public Guid AnchorType { get; set; }

            public PersonalInfo Attendee { get; set; }
        }

        private Dictionary<int, AnchorAssignment> anchors = new Dictionary<int, AnchorAssignment>();
        
        /// <summary>
        /// Initializes a new instance of the <see cref="SeatAssignments"/> class.
        /// </summary>
        /// <param name="orderId">The order id that triggers this anchor assignments creation.</param>
        /// <param name="anchors">The order anchors.</param>
        public AnchorAssignments(Guid orderId, IEnumerable<AnchorQuantity> anchors)
            // Note that we don't use the order id as the assignments id
            : this(GuidUtil.NewSequentialId())
        {
            // Add as many assignments as anchors there are.
            var i = 0;
            var all = new List<AnchorAssignmentsCreated.AnchorAssignmentInfo>();
            foreach (var anchorQuantity in anchors)
            {
                for (int j = 0; j < anchorQuantity.Quantity; j++)
                {
                    all.Add(new AnchorAssignmentsCreated.AnchorAssignmentInfo { Position = i++, SeatType = anchorQuantity.AnchorType });
                }
            }

            base.Update(new AnchorAssignmentsCreated { OrderId = orderId, Anchors = all });
        }

        public AnchorAssignments(Guid id, IEnumerable<IVersionedEvent> history)
            : this(id)
        {
            this.LoadFrom(history);
        }

        private AnchorAssignments(Guid id)
            : base(id)
        {
            base.Handles<AnchorAssignmentsCreated>(this.OnCreated);
            base.Handles<AnchorAssigned>(this.OnAnchorAssigned);
            base.Handles<AnchorUnassigned>(this.OnAnchorUnassigned);
            base.Handles<AnchorAssignmentUpdated>(this.OnAnchorAssignmentUpdated);
        }

        public void AssignAnchor(int position, PersonalInfo attendee)
        {
            if (string.IsNullOrEmpty(attendee.Email))
                throw new ArgumentNullException("attendee.Email");

            AnchorAssignment current;
            if (!this.anchors.TryGetValue(position, out current))
                throw new ArgumentOutOfRangeException("position");

            if (!attendee.Email.Equals(current.Attendee.Email, StringComparison.OrdinalIgnoreCase))
            {
                if (current.Attendee.Email != null)
                {
                    this.Update(new AnchorUnassigned(this.Id) { Position = position });
                }

                this.Update(new AnchorAssigned(this.Id)
                {
                    Position = position,
                    SeatType = current.AnchorType,
                    Attendee = attendee,
                });
            }
            else if (!string.Equals(attendee.FirstName, current.Attendee.FirstName, StringComparison.OrdinalIgnoreCase)
                || !string.Equals(attendee.LastName, current.Attendee.LastName, StringComparison.OrdinalIgnoreCase))
            {
                Update(new AnchorAssignmentUpdated(this.Id)
                {
                    Position = position,
                    Attendee = attendee,
                });
            }
        }

        public void Unassign(int position)
        {
            AnchorAssignment current;
            if (!this.anchors.TryGetValue(position, out current))
                throw new ArgumentOutOfRangeException("position");

            if (current.Attendee.Email != null)
            {
                this.Update(new AnchorUnassigned(this.Id) { Position = position });
            }
        }

        private void OnCreated(AnchorAssignmentsCreated e)
        {
            this.anchors = e.Anchors.ToDictionary(x => x.Position, x => new AnchorAssignment { Position = x.Position, AnchorType = x.SeatType });
        }

        private void OnAnchorAssigned(AnchorAssigned e)
        {
            this.anchors[e.Position] = Mapper.Map(e, new AnchorAssignment());
        }

        private void OnAnchorUnassigned(AnchorUnassigned e)
        {
            this.anchors[e.Position] = Mapper.Map(e, new AnchorAssignment { AnchorType = this.anchors[e.Position].AnchorType });
        }

        private void OnAnchorAssignmentUpdated(AnchorAssignmentUpdated e)
        {
            this.anchors[e.Position] = Mapper.Map(e, new AnchorAssignment
            {
                // Seat type is also never received again from the client.
                AnchorType = this.anchors[e.Position].AnchorType,
                // The email property is not received for updates, as those 
                // are for the same attendee essentially.
                Attendee = new PersonalInfo { Email = this.anchors[e.Position].Attendee.Email }
            });
        }
    }
}
