using System;
using System.Linq.Expressions;
using System.Web.Mvc;
using System.Web.Routing;
using ExpressionHelper = Microsoft.Web.Mvc.Internal.ExpressionHelper;

namespace FODT
{
    public static class ChildActionExtensions
    {
        public static void RenderAction<TController>(this HtmlHelper htmlHelper, Expression<Action<TController>> action) where TController : Controller
        {
            RouteValueDictionary routeValues = ExpressionHelper.GetRouteValuesFromExpression(action);
            System.Web.Mvc.Html.ChildActionExtensions.RenderAction(htmlHelper, routeValues["action"] as string, routeValues);
        }
    }
}