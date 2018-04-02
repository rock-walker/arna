namespace AP.Infrastructure.Azure.EventSourcing
{
    using System.Xml.Serialization;

    /// <summary>
    /// Simple settings class to configure the connection to Windows Azure tables.
    /// </summary>
    [XmlRoot("EventSourcing", Namespace = InfrastructureSettings.XmlNamespace)]
    public class EventSourcingSettings
    {
        /// <summary>
        /// Gets or sets the service URI scheme.
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// Gets or sets the name of the Windows Azure table used for the Orders and Seats Assignments Event Store.
        /// </summary>
        public string OrdersTableName { get; set; }

        /// <summary>
        /// Gets or sets the name of the Windows Azure table used for the Seats Availability Event Store.
        /// </summary>
        public string AnchorsAvailabilityTableName { get; set; }
    }
}
