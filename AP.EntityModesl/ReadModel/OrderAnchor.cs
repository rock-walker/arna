// ==============================================================================================================
using AP.Business.Model.Registration;

namespace AP.EntityModel.ReadModel
{

    public class OrderAnchor
    {
        public OrderAnchor()
        {
            this.Attendee = new PersonalInfo();
        }

        public OrderAnchor(int position, string seatName)
        {
            this.Position = position;
            this.SeatName = seatName;
            this.Attendee = new PersonalInfo();
        }

        public int Position { get; set; }
        public string SeatName { get; set; }
        public PersonalInfo Attendee { get; set; }
    }
}
