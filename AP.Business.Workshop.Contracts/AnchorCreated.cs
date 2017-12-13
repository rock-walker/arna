namespace AP.Business.AutoPortal.Events
{
    using System;
    using AP.Infrastructure.Messaging;

    /// <summary>
    /// Event raised when a new seat type is created. Note 
    /// that when a seat type is created.
    /// </summary>
    public class AnchorCreated : IEvent
    {
        /// <summary>
        /// Gets or sets the conference identifier.
        /// </summary>
        public Guid WorkshopId { get; set; }

        /// <summary>
        /// Gets or sets the source seat type identifier.
        /// </summary>
        public Guid SourceId { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
}
