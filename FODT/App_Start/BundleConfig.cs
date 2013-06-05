using System.Web.Optimization;
using dotless.Core;

namespace FODT
{
    public class BundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {
            // Stylesheets
            var css = new StyleBundle("~/Assets/css/site").Include(
                        "~/assets/css/bootstrap.css",
                        "~/assets/css/fodt-main.css",
                        "~/assets/css/fodt-forms.css",
                        "~/assets/css/fodt-tables.css",
                        "~/assets/css/fodt-slideshow.css"
                        );
            css.Transforms.Add(new LessTransform());
            bundles.Add(css);
        }

        public class LessTransform : IBundleTransform
        {
            public void Process(BundleContext context, BundleResponse response)
            {
                response.Content = Less.Parse(response.Content);
                response.ContentType = "text/css";
            }
        }
    }
}