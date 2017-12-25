namespace AP.ViewModel.Booking
{
    using System;
    using System.Collections.Generic;

    public class OrderViewModel
    {
        public OrderViewModel()
        {
            this.Items = new List<OrderItemViewModel>();
        }

        public Guid OrderId { get; set; }

        public int OrderVersion { get; set; }

        public Guid WorkshopId { get; set; }

        public string WorkshopCode { get; set; }

        public string WorkshopName { get; set; }

        public IList<OrderItemViewModel> Items { get; set; }

        public long ReservationExpirationDate { get; set; }
    }
}
