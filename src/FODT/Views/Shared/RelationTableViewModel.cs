using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;
using System.Web.Mvc.Html;

namespace FODT.Views.Shared
{
    public abstract class RelationTableViewModel : IUserEditableViewModel
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

        public List<RelationViewModel> Items { get; set; }
    }

    public abstract class RelationTableViewModel<T> : RelationTableViewModel
        where T : RelationViewModel
    {
    }

    public class RelationViewModel
    {
        public string DeleteItemURL { get; set; }
    }

    public static class RelationTableExtensions
    {
        public static MvcHtmlString RelationTable(this HtmlHelper htmlHelper, dynamic model, Func<dynamic, HelperResult> columnTemplate)
        {
            htmlHelper.ViewData["RelationTable_ColumnTemplate"] = columnTemplate;
            return htmlHelper.Partial("RelationTable", (object)model);
        }

        public static MvcHtmlString RelationTable<T>(this HtmlHelper htmlHelper, RelationTableViewModel<T> model, Func<T, HelperResult> columnTemplate)
            where T : RelationViewModel
        {
            htmlHelper.ViewData["RelationTable_ColumnTemplate"] = columnTemplate;
            return htmlHelper.Partial("RelationTable", model);
        }
    }
}