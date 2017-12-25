namespace AP.EntityModel.Booking
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Order
    {
        public enum OrderStatus
        {
            Pending,
            Paid,
        }

        public Order(Guid workshopId, Guid orderId, string accessCode)
            : this()
        {
            Id = orderId;
            WorkshopId = workshopId;
            AccessCode = accessCode;
            CategoryIds = "";
            AttendeeID = Guid.Empty;
            AutoID = Guid.Empty;
        }

        protected Order()
        {
            this.Anchors = new ObservableCollection<OrderAnchor>();
        }

        [Key]
        public Guid Id { get; set; }
        public Guid WorkshopId { get; set; }

        /// <summary>
        /// Used for correlating with the seat assignments.
        /// </summary>
        public Guid? AssignmentsId { get; set; }

        [Display(Name = "Order Code")]
        public string AccessCode { get; set; }

        [Display(Name = "Total Amount")]
        public decimal TotalAmount { get; set; }

        public string CategoryIds { get; set; }
        public Guid AutoID { get; set; }
        public Guid AttendeeID { get; set; }
        public string Description { get; set; }
        public DateTime? BookingTime { get; set; }

        /// <summary>
        /// This pattern is typical for EF 4 since it does 
        /// not support native enum persistence. EF 4.5 does.
        /// </summary>
        [NotMapped]
        public OrderStatus Status
        {
            get { return (OrderStatus)this.StatusValue; }
            set { this.StatusValue = (int)value; }
        }
        public int StatusValue { get; set; }

        public ICollection<OrderAnchor> Anchors { get; set; }
    }
}
