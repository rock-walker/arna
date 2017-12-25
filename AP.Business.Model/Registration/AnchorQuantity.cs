namespace AP.Business.Model.Registration
{
    using System;

    public struct AnchorQuantity
    {
        private Guid anchorType;
        private int quantity;

        public AnchorQuantity(Guid anchorType, int quantity)
        {
            this.anchorType = anchorType;
            this.quantity = quantity;
        }

        public Guid AnchorType
        {
            get { return anchorType; }
            set { this.anchorType = value; }
        }

        public int Quantity
        {
            get { return quantity; }
            set { quantity = value; }
        }
    }
}
