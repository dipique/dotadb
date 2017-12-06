using System.Linq;

using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

using DotAPicker.Models;

namespace DotAPicker
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new RequiresAuthAttribute());
        }
    }

    public class RequiresAuthAttribute: AuthorizeAttribute
    {
        public RequiresAuthAttribute() { }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary {
                { "action", "Login" },
                { "controller", "Login" }
            });
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            var user = (User)httpContext.Session["user"];
            if (user == null) return false;
            if (user.ProfileType == ProfileTypes.Public) return true;
            if (user.ProfileType == ProfileTypes.Private &&
                user.IsAuthenticated) return true;

            return false;
        }
    }
}
