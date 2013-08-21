using System.Web.Mvc;
using System.Web.Routing;
using FluentSecurity;

namespace BycicleShop.Code.FluentSecurity
{
    public class DefaultPolicyViolationHandler : IPolicyViolationHandler
    {
        public string ViewName = "AccessDenied";
        public ActionResult Handle(PolicyViolationException exception)
        {
            if (SecurityHelper.UserIsAuthenticated())
            {
                return new ViewResult { ViewName = ViewName };
            }
            var rvd = new RouteValueDictionary();

            if (System.Web.HttpContext.Current.Request.RawUrl != "/")
                rvd["ReturnUrl"] = System.Web.HttpContext.Current.Request.RawUrl;

            rvd["controller"] = "Home";
            rvd["action"] = "Index";
            rvd["area"] = "";

            return new RedirectToRouteResult(rvd);
        }
    }
}