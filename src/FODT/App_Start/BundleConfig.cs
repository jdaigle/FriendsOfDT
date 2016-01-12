using System.Web.Optimization;

namespace FODT
{
    public class BundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
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
    }
}