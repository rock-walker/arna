namespace AP.Infrastructure.Sql.EventSourcing
{
    using EntityFramework.DbContextScope.Interfaces;
    using Microsoft.EntityFrameworkCore;
    using System;

    /// <summary>
    /// Used by <see cref="SqlEventSourcedRepository{T}"/>, and is used only for running the sample application
    /// without the dependency to the Windows Azure Service Bus when using the DebugLocal solution configuration.
    /// </summary>
    public class EventStoreDbContext : DbContext, IDbContext
    {
        public const string SchemaName = "Events";

        public EventStoreDbContext(DbContextOptions<EventStoreDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Event>().HasKey(x => new { x.AggregateId, x.AggregateType, x.Version });
            modelBuilder.Entity<Event>().ToTable("Events", SchemaName);
        }
    }

    public class Event
    {
        public Guid AggregateId { get; set; }
        public string AggregateType { get; set; }
        public int Version { get; set; }
        public string Payload { get; set; }
        public string CorrelationId { get; set; }

        // TODO: Following could be very useful for when rebuilding the read model from the event store, 
        // to avoid replaying every possible event in the system
        // public string EventType { get; set; }
    }
}
