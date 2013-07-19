using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using BycicleShop.Common.SqlContext.Concrete;

namespace Itransition.Membership
{
    public sealed class WebSecurity
    {
        public static HttpContextBase Context
        {
            get { return new HttpContextWrapper(HttpContext.Current); }
        }

        public static HttpRequestBase Request
        {
            get { return Context.Request; }
        }

        public static HttpResponseBase Response
        {
            get { return Context.Response; }
        }

        public static System.Security.Principal.IPrincipal User
        {
            get { return Context.User; }
        }

        public static bool IsAuthenticated
        {
            get { return User.Identity.IsAuthenticated; }
        }

        public MembershipCreateStatus Register(string username, string password, string email, bool isApproved, string firstName, string lastName)
        {
            
            MembershipCreateStatus createStatus;
            System.Web.Security.Membership.CreateUser(username, password, email, null, null, isApproved, null, out createStatus);

            if (createStatus == MembershipCreateStatus.Success)
            {
                using (var context = new UsersDataContext())
                {
                    var user = context.Entity.FirstOrDefault(x => x.UserName == username);
                    if (user != null)
                    {
                        user.FirstName = firstName;
                        user.LastName = lastName;
                    }
                    context.SaveChanges(); 
                }
                
                if (isApproved)
                {
                    FormsAuthentication.SetAuthCookie(username, false);
                }
            }
            return createStatus;
        }

        public bool Login(string userName, string password, bool remeberMe)
        {
            if (System.Web.Security.Membership.ValidateUser(userName, password))
            {
                FormsAuthentication.SetAuthCookie(userName, false);
                return true;
            }
            return false;
        }

        public static void LogOut()
        {
            FormsAuthentication.SignOut();
        }

        public MembershipUser GetUser(string username, bool userIsOnline)
        {
            var customMembership = (global::Itransition.Membership.CustomMembershipProvider)System.Web.Security.Membership.Providers["CustomMembershipProvider"];
            return customMembership.GetUser(username, userIsOnline);
        }

        public bool ChangePassword(string oldPassword, string newPassword)
        {
            var customMembership = (global::Itransition.Membership.CustomMembershipProvider)System.Web.Security.Membership.Providers["CustomMembershipProvider"];
            var membershipUser = customMembership.GetUser(User.Identity.Name, true);
            return membershipUser != null && membershipUser.ChangePassword(oldPassword, newPassword);
        }

        public bool DeleteUser(string username)
        {
            var customMembership = (global::Itransition.Membership.CustomMembershipProvider)System.Web.Security.Membership.Providers["CustomMembershipProvider"];
            return customMembership.DeleteUser(username, true);
        }

        public List<MembershipUser> FindUsersByEmail(string email, int pageIndex, int pageSize)
        {
            var customMembership = (global::Itransition.Membership.CustomMembershipProvider)System.Web.Security.Membership.Providers["CustomMembershipProvider"];
            int totalRecords;
            return
                customMembership.FindUsersByEmail(email, pageIndex, pageSize, out totalRecords)
                          .Cast<MembershipUser>()
                          .ToList();
        }

        public List<MembershipUser> FindUsersByName(string username, int pageIndex, int pageSize)
        {
            var customMembership = (global::Itransition.Membership.CustomMembershipProvider)System.Web.Security.Membership.Providers["CustomMembershipProvider"];
            int totalRecords;
            return
                customMembership.FindUsersByName(username, pageIndex, pageSize, out totalRecords)
                          .Cast<MembershipUser>()
                          .ToList();
        }

        public List<MembershipUser> GetAllUsers(int pageIndex, int pageSize)
        {
            var customMembership = (global::Itransition.Membership.CustomMembershipProvider)System.Web.Security.Membership.Providers["CustomMembershipProvider"];
            int totalRecords;
            return customMembership.GetAllUsers(pageIndex, pageSize, out totalRecords).Cast<MembershipUser>().ToList();
        }

        public static int GetUserId(string username)
        {
            return 0;
        }
    }
}
