using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using FriendsOfDT.Tasks;
using Newtonsoft.Json;
using Raven.Client;
using Raven.Client.Document;
using RiaLibrary.Web;

namespace FriendsOfDT {
    public class MvcApplication : System.Web.HttpApplication {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters) {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes) {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("favicon.ico");
            routes.MapRoutes();
            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Public", action = "Index", id = UrlParameter.Optional } // Parameter defaults
            );
        }

        protected void Application_Start() {
            AreaRegistration.RegisterAllAreas();
            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);
            InitializeRavenDb();
        }

        protected void Application_EndRequest() {
            var context = new HttpContextWrapper(Context);
            // If we're an ajax request, and doing a 302, then we actually need to do a 401
            if (Context.Response.StatusCode == 302 && context.Request.IsAjaxRequest()) {
                Context.Response.Clear();
                Context.Response.StatusCode = 200;
                Context.Response.ContentType = "application/json";
                var serializer = new JsonSerializer();
                serializer.Serialize(Context.Response.Output, new { redirect = Context.Response.Headers["Location"] });
                //serializer.Serialize(Context.Response.Output, new { redirect = VirtualPathUtility.ToAbsolute("~/Accounts/Login") });
            }
        }

        public static IDocumentStore DocumentStore { get; private set; }

        public static void InitializeRavenDb() {
            var documentStore = new DocumentStore { Url = "http://localhost:8080" };
            documentStore.DefaultDatabase = "FriendsOfDT";
            documentStore.RegisterListener(new DocumentVersionStoreListener());
            documentStore.Initialize();
            var defaultBehavior = documentStore.Conventions.FindTypeTagName;
            documentStore.Conventions.FindTypeTagName = type => {
                if (typeof(BackgroundTask).IsAssignableFrom(type))
                    return defaultBehavior(typeof(BackgroundTask));
                return defaultBehavior(type);
            };
            DocumentStore = documentStore;
        }
    }
}