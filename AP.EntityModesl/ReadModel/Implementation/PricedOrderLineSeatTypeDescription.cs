namespace AP.EntityModel.ReadModel.Implementation
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class PricedOrderLineAnchorTypeDescription
    {
        [Key]
        public Guid SeatTypeID { get; set; }
        public string Name { get; set; }
    }
}
