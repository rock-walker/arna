using System;
using System.Collections.Generic;

namespace AP.EntityModel.AttendeeDomain
{
    public class AttendeeData
    {
        public Guid ID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Guid UserID { get; set; }
        public IEnumerable<AttendeeAutoData> AttendeeAutos { get; set; }
    }
}
