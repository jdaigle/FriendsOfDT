using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FODT.Views.Extensions
{
    public static class ViewModelExtensions
    {
        public static HtmlString RenderChecked(this object model, string property)
        {
            if ((bool)model.Reflection().GetPropertyValue(property))
            {
                return MvcHtmlString.Create("checked=\"checked\"");
            }
            return MvcHtmlString.Empty;
        }
    }
}