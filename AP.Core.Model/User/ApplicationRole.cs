using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System;

namespace AP.Core.Model.User
{
    public class ApplicationRole : IdentityRole<Guid>//, ApplicationUserRole, ApplicationRoleClaim>
    {
        public ApplicationRole(string rolename) : base(rolename) { }
        public ApplicationRole() : base() { }
    }
}
