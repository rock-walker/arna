namespace AP.EntityModel.ReadModel
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class DraftOrder
    {
        public enum States
        {
            PendingReservation = 0,
            PartiallyReserved = 1,
            ReservationCompleted = 2,
            Rejected = 3,
            Confirmed = 4,
        }

        public DraftOrder(Guid orderId, Guid workshopId, States state, int orderVersion = 0)
            : this()
        {
            OrderID = orderId;
            WorkshopID = workshopId;
            State = state;
            OrderVersion = orderVersion;
        }

        protected DraftOrder()
        {
            this.Lines = new ObservableCollection<DraftOrderItem>();
        }

        [Key]
        public Guid OrderID { get; private set; }

        public Guid WorkshopID { get; private set; }

        public DateTime? ReservationExpirationDate { get; set; }

        public ICollection<DraftOrderItem> Lines { get; private set; }

        public int StateValue { get; private set; }

        [NotMapped]
        public States State
        {
            get { return (States)this.StateValue; }
            set { this.StateValue = (int)value; }
        }

        public int OrderVersion { get; set; }
        public Guid AttendeeID { get; set; }
        public string AccessCode { get; set; }
    }
}
