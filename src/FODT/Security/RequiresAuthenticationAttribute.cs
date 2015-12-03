using System;
using System.Web;
using System.Web.Mvc;
using FODT.Controllers;

namespace FODT.Security
{
    public class RequiresAuthenticationAttribute : FilterAttribute, IAuthorizationFilter
    {
        public RequiresAuthenticationAttribute()
        {
            this.RequiredRoles = new string[0];
        }
        public RequiresAuthenticationAttribute(params string[] requiredRoles)
        {
            this.RequiredRoles = requiredRoles;
        }

        public string[] RequiredRoles { get; set; }

        public static Func<IAuthenticationTokenContext> GetAuthenticationTokenContext { get; set; }

        static RequiresAuthenticationAttribute()
        {
            GetAuthenticationTokenContext = new Func<IAuthenticationTokenContext>(() =>
            {
                var httpContext = new HttpContextWrapper(HttpContext.Current);
                return new HttpAuthenticationTokenContext(httpContext, new HttpCookieCollectionWrapper(httpContext));
            });
        }

        public void OnAuthorization(AuthorizationContext filterContext)
        {
            var currentPath = filterContext.HttpContext.Request.Url.AbsolutePath;
            if (filterContext.HttpContext.Request.IsAjaxRequest())
            {
                currentPath = string.Empty;
                if (filterContext.HttpContext.Request.UrlReferrer != null)
                {
                    currentPath = filterContext.HttpContext.Request.UrlReferrer.PathAndQuery;
                }
            }
            var user = filterContext.HttpContext.User;
            if (user == null || !user.Identity.IsAuthenticated)
            {
                // redirect to error page
                OnUnauthorized(filterContext, null);
                return;
            }
            foreach (var role in RequiredRoles ?? new string[0])
            {
                if (!user.IsInRole(role))
                {
                    // redirect to error page
                    OnUnauthorized(filterContext, null);
                    return;
                }
            }
        }

        private void OnUnauthorized(AuthorizationContext filterContext, ActionResult route)
        {
            if (filterContext.Result != null)
            {
                return;
            }
            //filterContext.Result = new RedirectToRouteResult(route.GetRouteValueDictionary());
            filterContext.Result = new ContentResult() { Content = "Authentication Required" };
        }
    }
}