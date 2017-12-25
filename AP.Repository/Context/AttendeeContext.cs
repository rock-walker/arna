using AP.EntityModel.AttendeeDomain;
using EntityFramework.DbContextScope.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AP.Repository.Context
{
    public class AttendeeContext : DbContext, IDbContext
    {
        private const string _schema = "Client";
        public AttendeeContext(DbContextOptions<AttendeeContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder model)
        {
            model.Entity<AttendeeData>().ToTable("Attendees", _schema);
            model.Entity<AttendeeAutoData>().ToTable("AttendeeAutos", _schema);
        }
    }
}
