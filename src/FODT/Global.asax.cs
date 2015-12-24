using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using FODT.Database;
using FODT.Infrastructure;
using StackExchange.Profiling;
using StackExchange.Profiling.Mvc;
using System.Linq;

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


            MiniProfiler.Settings.IgnoredPaths = new[]
            {
                "/assets",
                "/favicon.ico",
            };
            var copy = ViewEngines.Engines.ToList();
            ViewEngines.Engines.Clear();
            foreach (var item in copy)
            {
                ViewEngines.Engines.Add(new ProfilingViewEngine(item));
            }
        }

        protected void Application_BeginRequest()
        {
            MiniProfiler.Start();
        }

        protected void Application_EndRequest()
        {
            MiniProfiler.Stop();
        }
    }
}