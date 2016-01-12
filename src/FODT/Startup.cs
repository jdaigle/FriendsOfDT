using System.Linq;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using FODT.Database;
using FODT.Infrastructure;
using FODT.Security;
using StackExchange.Profiling;
using StackExchange.Profiling.Mvc;

namespace FODT
{
    public static class Startup
    {
        public static void ApplicationStart()
        {
            DatabaseBootstrapper.Bootstrap();

            MvcHandler.DisableMvcResponseHeader = true;

            //AreaRegistration.RegisterAllAreas();
            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);
            RegisterBundles(BundleTable.Bundles);

            ViewEngines.Engines.Insert(0, new ViewModelSpecifiedViewEngine());

            SetupMiniProfiler();
        }

        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            //filters.Add(new HandleErrorAttribute());
            filters.Add(new CookieAuthenticationFilter());
            filters.Add(new LegacyUrlFilter());
            filters.Add(new HttpContentNegotiationFilter());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("assets/{*pathInfo}");
            routes.IgnoreRoute("favicon.ico");
            routes.IgnoreRoute("elmah.axd");
            routes.IgnoreRoute("routes.axd");
            routes.IgnoreRoute("robots.txt");
            routes.MapMvcAttributeRoutes();
            routes.LowercaseUrls = true;
        }

        public static void RegisterBundles(BundleCollection bundles)
        {
            // Stylesheets
            var css = new StyleBundle("~/Assets/css/site").Include(
                        "~/assets/css/bootstrap.css"
                        , "~/assets/css/bootstrap-theme.css"
                        , "~/assets/css/fodt-shared.css"
                        , "~/assets/css/fodt-components.css"
                        , "~/assets/css/fodt-admin.css"
                        );
            bundles.Add(css);


            bundles.Add(new ScriptBundle("~/Assets/js/site").Include(
                        "~/assets/js/lib/jquery-2.2.0.min.js"
                        , "~/assets/js/lib/bootstrap.js"
                        , "~/assets/js/lib/typeahead.jquery.js"
                        , "~/assets/js/lib/jsrender.js"
                        , "~/assets/js/fodt.polyfill.js"
                        , "~/assets/js/fodt.common.js"
                        ));
        }

        private static void SetupMiniProfiler()
        {
            MiniProfiler.Settings.IgnoredPaths = new[]
                        {
                "/assets",
                "/favicon.ico",
                "/elmah.axd",
            };
            var copy = ViewEngines.Engines.ToList();
            ViewEngines.Engines.Clear();
            foreach (var item in copy)
            {
                ViewEngines.Engines.Add(new ProfilingViewEngine(item));
            }
        }
    }
}