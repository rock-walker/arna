namespace AP.Business.Registration
{
    using System;

    public class OrderItem
    {
        public OrderItem(Guid anchorType, int quantity)
        {
            this.AnchorType = anchorType;
            this.Quantity = quantity;
        }

        public Guid AnchorType { get; private set; }

        public int Quantity { get; private set; }
    }
}
