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
                        , "~/assets/css/fodt-components.css"
                //, "~/assets/css/fodt-main.css"
                //, "~/assets/css/fodt-forms.css"
                //, "~/assets/css/fodt-tables.css"
                //, "~/assets/css/fodt-slideshow.css"
                        );
            bundles.Add(css);


            bundles.Add(new ScriptBundle("~/Assets/js/site").Include(
                        "~/assets/js/lib/jquery-2.1.3.min.js"
                        , "~/assets/js/lib/bootstrap.js"
                        , "~/assets/js/lib/typeahead.jquery.js"
                        , "~/assets/js/lib/jsrender.js"
                        , "~/assets/js/fodt.polyfill.js"
                        , "~/assets/js/fodt.common.js"
                        ));
        }
    }
}