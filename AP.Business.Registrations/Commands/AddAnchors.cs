namespace AP.Business.Registration.Commands
{
    using System;

    /// <summary>
    /// Adds seats to an existing seat type.
    /// </summary>
    public class AddAnchors : AnchorsAvailabilityCommand
    {
        /// <summary>
        /// Gets or sets the type of the seat.
        /// </summary>
        public Guid AnchorType { get; set; }

        /// <summary>
        /// Gets or sets the quantity of seats added.
        /// </summary>
        public int Quantity { get; set; }
    }
}
