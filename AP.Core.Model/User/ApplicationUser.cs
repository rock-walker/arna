using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System;

namespace AP.Core.Model.User
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser<Guid>
    {
    }
}
