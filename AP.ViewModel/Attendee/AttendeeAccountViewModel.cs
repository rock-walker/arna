using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AP.ViewModel.Attendee
{
    public class AttendeeAccountViewModel
    {
        [StringLength(64)]
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public IEnumerable<AttendeeAutoViewModel> AttendeeAutos { get; set; }
    }
}
