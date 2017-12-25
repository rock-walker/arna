namespace AP.Business.Registration.Commands
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using AP.Infrastructure.Messaging;
    using System.Collections.Generic;

    public class AssignRegistrantDetails : ICommand
    {
        public AssignRegistrantDetails()
        {
            this.Id = Guid.NewGuid();
        }

        public Guid Id { get; private set; }
        public Guid OrderId { get; set; }
        public Guid AttendeeID { get; set; }
        public string Description { get; set; }
        public List<int> CategoryIds { get; set; }
        public DateTime? AssignedDate { get; set; }
        public Guid AutoID { get; set; }
    }
}
