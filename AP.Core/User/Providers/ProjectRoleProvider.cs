using System;
using System.Configuration.Provider;
using System.Linq;
using System.Web.Security;
using EL.EntityModels;

namespace AP.Core.User.Providers
{
    public class ProjectRoleProvider : RoleProvider
    {

        public class UserInRoleDto
        {
            public int UserId {get; set;}
            public int RoleId {get; set;}
        }

        public override void AddUsersToRoles(string[] emails, string[] roleNames)
        {
            foreach (string rolename in roleNames)
            {
                if (string.IsNullOrEmpty(rolename))
                    throw new ProviderException("Role name cannot be empty or null.");
                if (!RoleExists(rolename))
                    throw new ProviderException("Role name not found.");
            }

            foreach (string email in emails)
            {
                if (string.IsNullOrEmpty(email))
                    throw new ProviderException("User name cannot be empty or null.");
                if (email.Contains(","))
                    throw new ArgumentException("User names cannot contain commas.");

                if (roleNames.Any(rolename => IsUserInRole(email, rolename)))
                {
	                throw new ProviderException("User is already in role.");
                }
            }

            using (var _db = new UsersContext())
            {

                var profiles =  _db.UserProfiles.Where(
                                                    x => 
                                                    emails.Contains(x.Email));
               var role =     _db.UserRoles.FirstOrDefault(x => roleNames.Contains(x.RoleName));

                /*var emailsInRoles = (from p in profiles
									 where p.
                                    from r in roles
                                    select new UserInRoleDto {UserId = p.UserId, RoleId = r.RoleId}).Distinct();
				*/
                //if (!emailsInRoles.Any())
                //{
				if (role != null && profiles.Any())
                    foreach (var uInr in profiles)
                        _db.UserInRoles.Add(new UserInRole { UserId = uInr.UserId, RoleId = role.RoleId });

                    _db.SaveChanges();
                //}
            }
        }

        public override string ApplicationName
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public override void CreateRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            throw new NotImplementedException();
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            throw new NotImplementedException();
        }

        public override string[] GetAllRoles()
        {
            throw new NotImplementedException();
        }

        public override string[] GetRolesForUser(string username)
        {
            var role = new string[]{};

            using (var _db = new UsersContext())
            {
                try
                {
                    UserProfile user = (from u in _db.UserProfiles
                                        where u.UserName == username
                                        select u).FirstOrDefault();

                    if (user != null)
                    {
                        // получаем роль
                        var roles = from r in _db.UserInRoles
                                    from w in _db.UserRoles
                                    where r.UserId == user.UserId
                                    where w.RoleId == r.RoleId
                                    select w;

                        if (roles.Any())
                        {
                            role = roles.Select(x=>x.RoleName).ToArray();
                        }
                    }
                }
                catch
                {
                    role = new string[] { };
                }
            }

            return role;
        }

        public override string[] GetUsersInRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override bool IsUserInRole(string email, string roleName)
        {
            bool outputResult = false;
            using (var _db = new UsersContext())
            {
                try
                {
                    UserProfile user = (from u in _db.UserProfiles
                                 where u.Email == email
                                 select u).FirstOrDefault();
                    if (user != null)
                    {
                        var roles = from r in _db.UserInRoles
                                    from w in _db.UserRoles
                                    where r.UserId == user.UserId
                                    where w.RoleId == r.RoleId
                                    select w;


                        if (!roles.Any())
                            return false;

                        outputResult = roles.Any(x => x.RoleName == roleName);
                       
                    }
                }
                catch
                {
                    outputResult = false;
                }
            }
            return outputResult;
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override bool RoleExists(string roleName)
        {
            if (string.IsNullOrEmpty(roleName))
                throw new ProviderException("Role name cannot be empty or null.");

            bool exists = false;

            using (var _db = new UsersContext())
            {
                exists = _db.UserRoles.Select(m => m.RoleName == roleName).ToList().Count > 0;
            }

            return exists;
        }
    }
}