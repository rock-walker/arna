namespace AP.Repository.Context
{
    using System;
    using System.Linq;
    //using Microsoft.Practices.EnterpriseLibrary.WindowsAzure.TransientFaultHandling.SqlAzure;
    //using Microsoft.Practices.TransientFaultHandling;
    using AP.EntityModel.ReadModel;
    using AP.EntityModel.ReadModel.Implementation;
    using Microsoft.EntityFrameworkCore;
    using EntityFramework.DbContextScope.Interfaces;

    /// <summary>
    /// A repository stored in a database for the views.
    /// </summary>
    public class WorkshopRegistrationDbContext : DbContext, IDbContext
    {
        public const string SchemaName = "WorkshopRegistration";
        //private readonly RetryPolicy<SqlAzureTransientErrorDetectionStrategy> retryPolicy;

        public WorkshopRegistrationDbContext(DbContextOptions<WorkshopRegistrationDbContext> options) : base(options)
        {
            /*this.retryPolicy = new RetryPolicy<SqlAzureTransientErrorDetectionStrategy>(new Incremental(3, TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(1.5)) { FastFirstRetry = true });
            this.retryPolicy.Retrying += (s, e) =>
                Trace.TraceWarning("An error occurred in attempt number {1} to access the ConferenceRegistrationDbContext: {0}", e.LastException.Message, e.CurrentRetryCount);*/
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Make the name of the views match exactly the name of the corresponding property.
            modelBuilder.Entity<DraftOrder>().ToTable("OrdersView", SchemaName);
            modelBuilder.Entity<DraftOrder>().HasMany(c => c.Lines);//.WithRequired();
            modelBuilder.Entity<DraftOrderItem>().ToTable("OrderItemsView", SchemaName);
            modelBuilder.Entity<DraftOrderItem>().HasKey(item => new { item.OrderID, item.AnchorType });
            modelBuilder.Entity<PricedOrder>().ToTable("PricedOrders", SchemaName);
            modelBuilder.Entity<PricedOrder>().HasMany(c => c.Lines);//.WithRequired().HasForeignKey(x => x.OrderId);
            modelBuilder.Entity<PricedOrderLine>().ToTable("PricedOrderLines", SchemaName);
            modelBuilder.Entity<PricedOrderLine>().HasKey(seat => new { seat.OrderId, seat.Position });
            modelBuilder.Entity<PricedOrderLineAnchorTypeDescription>().ToTable("PricedOrderLineAnchorTypeDescriptions", SchemaName);

            modelBuilder.Entity<WorkshopView>().ToTable("WorkshopsView", SchemaName);
            modelBuilder.Entity<AnchorType>().ToTable("WorkshopAnchorTypesView", SchemaName);
        }

        public T Find<T>(Guid id) where T : class
        {
            //return this.retryPolicy.ExecuteAction(() => Set<T>().Find(id));
            return this.Set<T>().Find(id);
        }

        public IQueryable<T> Query<T>() where T : class
        {
            return this.Set<T>();
        }

        public void Save<T>(T entity) where T : class
        {
            var entry = this.Entry(entity);

            if (entry.State == EntityState.Detached)
                this.Set<T>().Add(entity);

            //retryPolicy.ExecuteAction(() => this.SaveChanges());
            SaveChanges();
        }
    }
}
