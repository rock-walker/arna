namespace AP.Business.Workshop.Contracts
{
    using System;
    using Infrastructure.Messaging;

    /// <summary>
    /// Event published whenever a previously public conference 
    /// is made private by unpublishing it.
    /// </summary>
    public class WorkshopUnpublished : IEvent
    {
        public Guid SourceId { get; set; }
    }
}
