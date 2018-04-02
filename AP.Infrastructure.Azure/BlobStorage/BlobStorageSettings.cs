using System.Xml.Serialization;

namespace AP.Infrastructure.Azure.BlobStorage
{
    /// <summary>
    /// Simple settings class to configure the connection to Windows Azure blobs.
    /// </summary>
    [XmlRoot("BlobStorage", Namespace = InfrastructureSettings.XmlNamespace)]
    public class BlobStorageSettings
    {
        /// <summary>
        /// Gets or sets the service URI scheme.
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// Gets or sets the name of the Windows Azure blob container used for read models.
        /// </summary>
        public string RootContainerName { get; set; }
    }
}
