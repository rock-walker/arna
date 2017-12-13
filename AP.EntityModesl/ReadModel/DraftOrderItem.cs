namespace AP.EntityModel.ReadModel
{
    using System;

    public class DraftOrderItem
    {
        public DraftOrderItem(Guid anchorType, int requestedAnchors)
        {
            AnchorType = anchorType;
            RequestedAnchors = requestedAnchors;
        }

        protected DraftOrderItem()
        {
        }

        public Guid OrderID { get; private set; }
        public Guid AnchorType { get; set; }
        public int RequestedAnchors { get; set; }
        public int ReservedAnchors { get; set; }
    }
}
