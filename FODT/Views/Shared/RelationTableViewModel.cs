using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;

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

        public abstract Func<RelationViewModel, HelperResult> RenderItemColumns { get; }
        public List<RelationViewModel> Items { get; set; }
    }

    public class RelationViewModel
    {
        public string DeleteItemURL { get; set; }
    }
}