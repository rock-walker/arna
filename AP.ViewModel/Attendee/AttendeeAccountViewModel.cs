using AP.ViewModel.Account;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AP.ViewModel.Attendee
{
    public class AttendeeAccountViewModel : IdentityViewModel
    {
        [StringLength(64)]
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public IEnumerable<AttendeeAutoViewModel> AttendeeAutos { get; set; }
    }
}
