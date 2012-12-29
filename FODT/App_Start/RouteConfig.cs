using System.Web.Mvc;
using System.Web.Routing;
using AttributeRouting.Web.Mvc;

namespace FODT
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("favicon.ico");
            routes.IgnoreRoute("elmah.axd");
            routes.IgnoreRoute("routes.axd");
            routes.MapAttributeRoutes(x =>
            {
                x.ScanAssembly(typeof(MvcApplication).Assembly);
                x.UseLowercaseRoutes = true;
            });
        }
    }
}