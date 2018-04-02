namespace AP.Infrastructure.Azure.MessageLog
{
    using System.Xml.Serialization;

    /// <summary>
    /// Simple settings class to configure the connection to Windows Azure tables.
    /// </summary>
    [XmlRoot("MessageLog", Namespace = InfrastructureSettings.XmlNamespace)]
    public class MessageLogSettings
    {
        /// <summary>
        /// Gets or sets the service URI scheme.
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// Gets or sets the name of the Windows Azure table used for the message log.
        /// </summary>
        public string TableName { get; set; }
    }
}
