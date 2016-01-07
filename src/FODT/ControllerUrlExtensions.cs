using System;
using System.Linq.Expressions;
using System.Web.Mvc;
using System.Web.Routing;
using ExpressionHelper = Microsoft.Web.Mvc.Internal.ExpressionHelper;

namespace FODT
{
    public static class ControllerUrlExtensions
    {
        public static RedirectToRouteResult RedirectToAction<TController>(this TController controller, Expression<Action<TController>> action, bool permanent = false) where TController : Controller
        {
            return RedirectToAction((Controller)controller, action, permanent);
        }

        public static RedirectToRouteResult RedirectToAction<TController>(this Controller controller, Expression<Action<TController>> action, bool permanent = false) where TController : Controller
        {
            if (controller == null)
            {
                throw new ArgumentNullException("controller");
            }

            RouteValueDictionary routeValues = ExpressionHelper.GetRouteValuesFromExpression(action);
            return new RedirectToRouteResult("", routeValues, permanent);
        }

        public static string GetURL<TController>(this UrlHelper urlHelper, Expression<Action<TController>> action) where TController : Controller
        {
            return urlHelper.Action<TController>(action);
        }

        public static string GetURL<TController>(this TController controller, Expression<Action<TController>> action) where TController : Controller
        {
            return GetURL((Controller)controller, action);
        }

        public static string GetURL<TController>(this Controller controller, Expression<Action<TController>> action) where TController : Controller
        {
            if (controller == null)
            {
                throw new ArgumentNullException("controller");
            }
            return controller.Url.GetURL<TController>(action);
        }

        public static UrlHelper Url(this ControllerContext context)
        {
            return context.Controller.Url();
        }

        public static UrlHelper Url(this ControllerBase controller)
        {
            if (controller is Controller)
            {
                return ((Controller)controller).Url;
            }
            var urlHelper = controller.ControllerContext.HttpContext.Items["_cachedUrlHelper"] as UrlHelper;
            if (urlHelper == null)
            {
                urlHelper = new UrlHelper(controller.ControllerContext.RequestContext);
                controller.ControllerContext.HttpContext.Items["_cachedUrlHelper"] = urlHelper;
            }
            return urlHelper;
        }
    }
}