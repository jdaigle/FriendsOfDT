using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Web.Mvc;
using System.Web.Routing;
using ExpressionHelper = Microsoft.Web.Mvc.Internal.ExpressionHelper;

namespace FODT
{
    public static class ControllerExtensions
    {
        public static RedirectToRouteResult RedirectToAction<TController>(this TController controller, Expression<Action<TController>> action) where TController : Controller
        {
            return RedirectToAction((Controller)controller, action);
        }

        public static RedirectToRouteResult RedirectToAction<TController>(this Controller controller, Expression<Action<TController>> action) where TController : Controller
        {
            if (controller == null)
            {
                throw new ArgumentNullException("controller");
            }

            RouteValueDictionary routeValues = ExpressionHelper.GetRouteValuesFromExpression(action);
            return new RedirectToRouteResult(routeValues);
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
            return new UrlHelper(context.RequestContext);
        }

        public static UrlHelper Url(this ControllerBase controller)
        {
            return new UrlHelper(controller.ControllerContext.RequestContext);
        }
    }
}