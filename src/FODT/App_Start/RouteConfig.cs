using System.Web.Mvc;
using System.Web.Routing;

namespace FODT
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("assets/{*pathInfo}");
            routes.IgnoreRoute("favicon.ico");
            routes.IgnoreRoute("elmah.axd");
            routes.IgnoreRoute("routes.axd");
            routes.IgnoreRoute("robots.txt");
            routes.MapMvcAttributeRoutes();
        }
    }
}