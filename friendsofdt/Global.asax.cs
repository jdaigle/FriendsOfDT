using System.Web.Mvc;
using System.Web.Routing;
using FriendsOfDT.Controllers;
using Raven.Client;
using Raven.Client.Document;
using StructureMap;
using FriendsOfDT.Tasks;

namespace FriendsOfDT {
    public class MvcApplication : System.Web.HttpApplication {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters) {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes) {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

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
            //InitializeStructureMap();
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

//        public static void InitializeStructureMap() {
//            var container = new Container(i => {
//                i.Scan(s => s.IncludeNamespaceContainingType<PublicController>());
//                i.ForSingletonOf<IDocumentStore>().Use(() => MvcApplication.DocumentStore);
//            });
//#if DEBUG
//            container.AssertConfigurationIsValid();
//#endif
//            // Register the container with ASP.NET MVC
//            ControllerBuilder.Current.SetControllerFactory(new StructureMapControllerFactory(container));
//        }
    }
}