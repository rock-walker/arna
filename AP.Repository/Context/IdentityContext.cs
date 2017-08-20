using AP.Core.Model.User;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;

namespace AP.Repository.Context
{
    public class IdentityContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>//, ApplicationUserClaim, 
        //ApplicationUserRole, ApplicationUserLogin, ApplicationRoleClaim, ApplicationUserToken>
    {
        public IdentityContext(DbContextOptions<IdentityContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<ApplicationUser>().ToTable("Users");
            builder.Entity<ApplicationRole>().ToTable("Roles");
            builder.Entity<IdentityUserRole<Guid>>().ToTable("UserRoles");
            builder.Entity<ApplicationUserLogin>().ToTable("UserLogins");
            builder.Entity<IdentityUserClaim<Guid>>().ToTable("UserClaims");
            builder.Entity<ApplicationRoleClaim>().ToTable("RoleClaims");
            builder.Entity<ApplicationUserToken>().ToTable("UserTokens");
        }
    }
}
