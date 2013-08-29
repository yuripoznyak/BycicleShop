using System.Collections.Generic;
using System.Web;
using System.Web.Security;
using BycicleShop.Controllers;
using FluentSecurity;

namespace BycicleShop.Code.FluentSecurity
{
    public class SecurityHelper
    {
        public static ISecurityConfiguration SetupFluentSecurity()
        {
            SecurityConfigurator.Configure(configuration =>
            {
                configuration.GetAuthenticationStatusFrom(() => HttpContext.Current.User.Identity.IsAuthenticated);
                configuration.GetRolesFrom(UserRoles);

                configuration.For<HomeController>().Ignore();
                
                configuration.For<AccountController>().DenyAuthenticatedAccess();
                configuration.For<AccountController>(x => x.LogOff()).DenyAnonymousAccess();
                configuration.For<AccountController>(x => x.Manage()).DenyAnonymousAccess();
                configuration.For<AccountController>(x => x.Account()).DenyAnonymousAccess();

                configuration.For<HelperController>().Ignore();

                configuration.For<OrderController>().DenyAnonymousAccess();

                configuration.For<AdminController>().DenyAnonymousAccess();
                configuration.For<AdminController>().RequireRole("Admin");

                configuration.For<StoreManagerController>().Ignore();

                configuration.IgnoreMissingConfiguration();
            });

            return SecurityConfiguration.Current;
        }

        public static bool UserIsAuthenticated()
        {
            var currentUser = HttpContext.Current.User;
            return !string.IsNullOrEmpty(currentUser.Identity.Name);
        }

        public static IEnumerable<object> UserRoles()
        {
            var currentUser = HttpContext.Current.User;
            return string.IsNullOrEmpty(currentUser.Identity.Name) ? null : System.Web.Security.Roles.GetRolesForUser(currentUser.Identity.Name);
        }

    }
}