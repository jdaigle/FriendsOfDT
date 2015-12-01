using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;

namespace FODT.Views
{
    public interface IUserEditableViewModel
    {
        bool CanEdit { get; }
    }

    public static class UserEditableViewModelExtensions
    {
        public static MvcHtmlString RenderIfCanEdit(this IUserEditableViewModel model, Func<object, HelperResult> template)
        {
            if (model.CanEdit)
            {
                return MvcHtmlString.Create(template.Invoke(null).ToHtmlString());
            }
            else
            {
                return MvcHtmlString.Empty;
            }
        }
    }
}