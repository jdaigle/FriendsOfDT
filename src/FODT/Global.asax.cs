using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using FODT.Database;
using FODT.Infrastructure;

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

            ViewEngines.Engines.Insert(0, new ViewModelSpecifiedViewEngine());
        }

        protected void Application_EndRequest()
        {
        }
    }
}