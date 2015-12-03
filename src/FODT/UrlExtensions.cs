using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Web.Mvc;
using System.Web.Routing;
using ExpressionHelper = Microsoft.Web.Mvc.Internal.ExpressionHelper;

namespace FODT
{
    public static class UrlExtensions
    {
        public static string Action<TController>(this UrlHelper url, Expression<Action<TController>> action) where TController : Controller
        {
            if (url == null)
            {
                throw new ArgumentNullException("url");
            }

            RouteValueDictionary routeValues = ExpressionHelper.GetRouteValuesFromExpression(action);
            return url.RouteUrl(routeValues);
        }
    }
}