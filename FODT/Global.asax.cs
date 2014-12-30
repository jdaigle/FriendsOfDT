using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using FODT.Database;
using FODT.Models;

namespace FODT
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            DatabaseBootstrapper.Bootstrap();
            MvcHandler.DisableMvcResponseHeader = true;
            //AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        protected void Application_EndRequest()
        {
            ObjectDisposal.DisposeAll();
        }
    }
}