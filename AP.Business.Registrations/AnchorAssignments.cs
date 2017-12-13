// ==============================================================================================================
// Microsoft patterns & practices
// CQRS Journey project
// ==============================================================================================================
// ©2012 Microsoft. All rights reserved. Certain content used with permission from contributors
// http://go.microsoft.com/fwlink/p/?LinkID=258575
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance 
// with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0
// Unless required by applicable law or agreed to in writing, software distributed under the License is 
// distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. 
// See the License for the specific language governing permissions and limitations under the License.
// ==============================================================================================================

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
    /// In our current business logic, 1 seats assignments instance corresponds to 1 <see cref="Order"/> instance. 
    /// This does not need to be the case in the future.
    /// <para>For more information on the domain, see <see cref="http://go.microsoft.com/fwlink/p/?LinkID=258553">Journey chapter 3</see>.</para>
    /// </remarks>
    public class AnchorAssignments : EventSourced
    {
        private class AnchorAssignment
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
        /*
        static AnchorAssignments()
        {
            Mapper.Initialize(cfg => cfg.CreateMap<AnchorAssigned, AnchorAssignments>());
            Mapper.Initialize(cfg => cfg.CreateMap<AnchorUnassigned, AnchorAssignments>());
            Mapper.Initialize(cfg => cfg.CreateMap<AnchorAssignmentUpdated, AnchorAssignments>());
        }
        */
        /// <summary>
        /// Initializes a new instance of the <see cref="SeatAssignments"/> class.
        /// </summary>
        /// <param name="orderId">The order id that triggers this seat assignments creation.</param>
        /// <param name="seats">The order seats.</param>
        public AnchorAssignments(Guid orderId, IEnumerable<AnchorQuantity> seats)
            // Note that we don't use the order id as the assignments id
            : this(GuidUtil.NewSequentialId())
        {
            // Add as many assignments as seats there are.
            var i = 0;
            var all = new List<AnchorAssignmentsCreated.AnchorAssignmentInfo>();
            foreach (var seatQuantity in seats)
            {
                for (int j = 0; j < seatQuantity.Quantity; j++)
                {
                    all.Add(new AnchorAssignmentsCreated.AnchorAssignmentInfo { Position = i++, SeatType = seatQuantity.AnchorType });
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
            base.Handles<AnchorAssigned>(this.OnSeatAssigned);
            base.Handles<AnchorUnassigned>(this.OnSeatUnassigned);
            base.Handles<AnchorAssignmentUpdated>(this.OnSeatAssignmentUpdated);
        }

        public void AssignSeat(int position, PersonalInfo attendee)
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

        private void OnSeatAssigned(AnchorAssigned e)
        {
            this.anchors[e.Position] = Mapper.Map(e, new AnchorAssignment());
        }

        private void OnSeatUnassigned(AnchorUnassigned e)
        {
            this.anchors[e.Position] = Mapper.Map(e, new AnchorAssignment { AnchorType = this.anchors[e.Position].AnchorType });
        }

        private void OnSeatAssignmentUpdated(AnchorAssignmentUpdated e)
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
