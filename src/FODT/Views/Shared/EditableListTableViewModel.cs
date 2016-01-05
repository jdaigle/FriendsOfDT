using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;
using System.Web.Mvc.Html;
using FODT.Views.Extensions;

namespace FODT.Views.Shared
{
    public abstract class EditableListTableViewModel : IUserEditableViewModel
    {
        public bool ShowTable
        {
            get
            {
                return CanEdit || Items.Any();
            }
        }

        public bool CanEdit { get; set; }

        public virtual string TableTitle { get; set; }
        public virtual string AddItemURL { get; set; }
        public virtual string AddItemURLText { get; set; }

        public List<EditableListViewModel> Items { get; set; }
    }

    public abstract class EditableListTableViewModel<T> : EditableListTableViewModel
        where T : EditableListViewModel
    {
    }

    public class EditableListViewModel
    {
        public string DeleteItemURL { get; set; }
    }

    public static class EditableListTableExtensions
    {
        public static MvcHtmlString EditableListTable(this HtmlHelper htmlHelper, dynamic model, Func<dynamic, HelperResult> columnTemplate)
        {
            htmlHelper.ViewData["EditableListTable_ColumnTemplate"] = columnTemplate;
            return htmlHelper.PartialView((object)model);
        }

        public static MvcHtmlString EditableListTable<T>(this HtmlHelper htmlHelper, EditableListTableViewModel<T> model, Func<T, HelperResult> columnTemplate)
            where T : EditableListViewModel
        {
            htmlHelper.ViewData["EditableListTable_ColumnTemplate"] = columnTemplate;
            return htmlHelper.PartialView(model);
        }
    }
}