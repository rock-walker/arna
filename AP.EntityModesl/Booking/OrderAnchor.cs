namespace AP.EntityModel.Booking
{
    using System;
    using System.ComponentModel.DataAnnotations.Schema;

    public class OrderAnchor
    {
        public OrderAnchor(Guid orderId, int position, Guid anchorId)
            : this()
        {
            this.OrderID = orderId;
            this.Position = position;
            this.AnchorID= anchorId;
            
        }

        public OrderAnchor()
        {
            this.Attendee_FirstName = "";
            this.Attendee_LastName = "";
            this.Attendee_Email = null;
        }

        public int Position { get; set; }
        public Guid OrderID { get; set; }
        public string Attendee_FirstName { get; set; }
        public string Attendee_LastName { get; set; }
        public string Attendee_Email { get; set; }

        /// <summary>
        /// Typical pattern for foreign key relationship 
        /// in EF. The identifier is all that's needed 
        /// to persist the referring entity.
        /// </summary>
        [ForeignKey("AnchorID")]
        public Guid AnchorID { get; set; }
        //public AnchorType AnchorInfo { get; set; }
    }
}
