using Microsoft.EntityFrameworkCore;

namespace AP.Repository.Context
{
	public class BaseContext<TContext, TMigration>: DbContext 
		where TContext : DbContext 
		//where TMigration: DbMigrationsConfiguration<TContext>, new()
	{
		static BaseContext()
		{
			//Database.SetInitializer();
			//Database.SetInitializer(new MigrateDatabaseToLatestVersion<TContext, TMigration>());
		}

		protected BaseContext() { }
	}

	public class BaseContext : DbContext
	{
        private readonly DbContextOptions<BaseContext> options;
        public BaseContext(DbContextOptions<BaseContext> options) : base(options)
        {

        }
	}
}
