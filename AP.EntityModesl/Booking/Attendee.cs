using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AP.EntityModel.Booking
{
    /// <summary>
    /// Represents an attendee to the conference, someone who has been 
    /// assigned to a purchased seat.
    /// </summary>
    [ComplexType]
    public class Attendee
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        // NOTE: we validate incoming data (this is filled from an event coming 
        // from the registration BC) so that when EF saves it will fail if it's invalid.
        [Key]
        [RegularExpression(@"[\w-]+(\.?[\w-])*\@[\w-]+(\.[\w-]+)+")]
        public string Email { get; set; }
    }
}
