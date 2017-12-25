namespace AP.Business.Workshop.Contracts
{
    using System;
    using Infrastructure.Messaging;

    /// <summary>
    /// Event published whenever a conference is made public.
    /// </summary>
    public class WorkshopPublished : IEvent
    {
        public Guid SourceId { get; set; }
    }
}
