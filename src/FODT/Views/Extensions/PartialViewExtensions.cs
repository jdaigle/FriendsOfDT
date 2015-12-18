using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace FODT.Views.Extensions
{
    public static class PartialViewExtensions
    {
        public static MvcHtmlString PartialView(this HtmlHelper htmlHelper, object model)
        {
            // minor hack, since all the internals of finding a partial view require a partialViewName
            return htmlHelper.Partial("null", model);
        }
    }
}