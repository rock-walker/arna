namespace AP.EntityModel.ReadModel
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class AnchorType
    {
        public AnchorType(Guid id, Guid workshopId, string name, string description, decimal price, int quantity)
        {
            this.ID = id;
            this.WorkshopID = workshopId;
            this.Name = name;
            this.Description = description;
            this.Price = price;
            this.Quantity = quantity;
            this.AvailableQuantity = 0;
            AnchorsAvailabilityVersion = -1;
        }

        protected AnchorType()
        {
        }

        [Key]
        public Guid ID { get; set; }

        // Conference ID is not FK, as we are relaxing the constraint due to eventual consistency
        public Guid WorkshopID { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public int AvailableQuantity { get; set; }
        public int AnchorsAvailabilityVersion { get; set; }
    }
}
