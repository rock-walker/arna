namespace AP.Infrastructure.Sql.MessageLog
{
    using EntityFramework.DbContextScope.Interfaces;
    using Microsoft.EntityFrameworkCore;

    public class MessageLogDbContext : DbContext, IDbContext
    {
        public const string SchemaName = "MessageLog";

        public MessageLogDbContext(DbContextOptions<MessageLogDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<MessageLogEntity>().ToTable("Messages", SchemaName);
        }
    }
}
