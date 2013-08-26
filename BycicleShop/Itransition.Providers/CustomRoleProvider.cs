using System.Linq;
using System.Web.Security;
using BycicleShop.Common.SqlContext.Concrete;
using BycicleShop.Common.SqlContext.Entities;

namespace Itransition.Providers
{
    public class CustomRoleProvider : RoleProvider
    {
        private RoleDataContext _roleDataContext = new RoleDataContext();
        private UsersDataContext _usersDataContext = new UsersDataContext();
        private UserInRoleDataContext _userinroleDataContext = new UserInRoleDataContext();

        public override string ApplicationName
        {
            get
            {
                return GetType().Assembly.GetName().Name;
            }
            set
            {
                ApplicationName = GetType().Assembly.GetName().Name;
            }
        }

        public override bool IsUserInRole(string username, string roleName)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(roleName))
            {
                return false;
            }

            User user;
            Role role;
            try
            {
                user = _usersDataContext.Entity.FirstOrDefault(x => x.UserName == username);
                role = _roleDataContext.Entity.FirstOrDefault(x => x.Name == roleName);
            }
            catch
            {
                return false;
            }
            if (user == null || role == null)
            {
                return false;
            }
            return user.UserInRoles.Any(x => user.UserId == x.UserId && role.RoleId == x.RoleId);
        }

        public override string[] GetRolesForUser(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                return null;
            }
            var user = _usersDataContext.Entity.FirstOrDefault(x => x.UserName == username);
            return user != null
                       ? _userinroleDataContext.Entity.Where(x => x.UserId == user.UserId)
                                               .Select(x => x.Role.Name)
                                               .ToArray()
                       : null;
        }

        public override void CreateRole(string roleName)
        {
            throw new System.NotImplementedException();
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            if (string.IsNullOrEmpty(roleName))
            {
                return false;
            }

            var role = _roleDataContext.Entity.FirstOrDefault(x => x.Name == roleName);
            if (role == null)
            {
                return false;
            }
            if (throwOnPopulatedRole)
            {
                if (_userinroleDataContext.Entity.Any(x => x.RoleId == role.RoleId))
                {
                    return false;
                }
            }
            else
            {
                var users = _userinroleDataContext.Entity.Where(x => x.RoleId == role.RoleId);
                foreach (var userInRole in users)
                {
                    _userinroleDataContext.Entity.Remove(userInRole);
                }
            }
            _userinroleDataContext.SaveChanges();
            return true;
        }

        public override bool RoleExists(string roleName)
        {
            if (string.IsNullOrEmpty(roleName))
            {
                return false;
            }

            Role role = null;
            try
            {
                role = _roleDataContext.Entity.FirstOrDefault(x => x.Name == roleName);
            }
            catch
            {
                return role != null;
            }
            return role != null;
        }

        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            /*using (DataContext Context = new DataContext())
            {
                List<User> Users = Context.Users.Where(Usr => usernames.Contains(Usr.Username)).ToList();
                List<Role> Roles = Context.Roles.Where(Rl => roleNames.Contains(Rl.RoleName)).ToList();
                foreach (User user in Users)
                {
                    foreach (Role role in Roles)
                    {
                        if (!user.Roles.Contains(role))
                        {
                            user.Roles.Add(role);
                        }
                    }
                }
                Context.SaveChanges();
            }*/
            throw new System.NotImplementedException();
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            /*using (DataContext Context = new DataContext())
            {
                foreach (String username in usernames)
                {
                    String us = username;
                    User user = Context.Users.FirstOrDefault(U => U.Username == us);
                    if (user != null)
                    {
                        foreach (String roleName in roleNames)
                        {
                            String rl = roleName;
                            Role role = user.Roles.FirstOrDefault(R => R.RoleName == rl);
                            if (role != null)
                            {
                                user.Roles.Remove(role);
                            }
                        }
                    }
                }
                Context.SaveChanges();
            }*/
            throw new System.NotImplementedException();
        }

        public override string[] GetUsersInRole(string roleName)
        {
            if (string.IsNullOrEmpty(roleName))
            {
                return null;
            }
            var role = _roleDataContext.Entity.FirstOrDefault(x => x.Name == roleName);
            return role != null
                       ? _userinroleDataContext.Entity.Where(x => x.RoleId == role.RoleId)
                                               .Select(x => x.User.UserName)
                                               .ToArray()
                       : null;
        }

        public override string[] GetAllRoles()
        {
            return _roleDataContext.Entity.Select(x => x.Name).ToArray();
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            if (string.IsNullOrEmpty(roleName) || string.IsNullOrEmpty(usernameToMatch))
            {
                return null;
            }

            return
                _userinroleDataContext.Entity.Where(
                    x => x.User.UserName.Contains(usernameToMatch) && x.Role.Name == roleName)
                                      .Select(x => x.User.UserName)
                                      .ToArray();
        }
    }
}
