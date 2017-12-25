namespace AP.Business.Registration.Commands
{
    using System;
    using AP.Infrastructure.Messaging;
    using AP.Business.Model.Registration;

    public class AssignAnchor : ICommand
    {
        public AssignAnchor()
        {
            this.Id = Guid.NewGuid();
        }

        public Guid Id { get; set; }

        public Guid SeatAssignmentsId { get; set; }
        public int Position { get; set; }
        public PersonalInfo Attendee { get; set; }
    }
}
