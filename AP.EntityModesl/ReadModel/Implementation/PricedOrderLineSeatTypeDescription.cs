namespace AP.EntityModel.ReadModel.Implementation
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class PricedOrderLineSeatTypeDescription
    {
        [Key]
        public Guid SeatTypeID { get; set; }
        public string Name { get; set; }
    }
}
