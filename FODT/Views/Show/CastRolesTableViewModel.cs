using System;
using System.Linq;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.WebPages;
using FODT.Models.IMDT;
using FODT.Views.Shared;
using FODT.Models;
using FODT.Controllers;

namespace FODT.Views.Show
{
    public class CastRolesTableViewModel : RelationTableViewModel
    {
        public CastRolesTableViewModel(UrlHelper url, Func<int, string> getDeleteItemURL, List<ShowCast> cast)
        {
            TableTitle = "Cast Credits";
            AddItemURLText = "Add Cast Credit";

            Items = new List<RelationViewModel>();
            Items.AddRange(cast
                .OrderByDescending(x => x.Show.Year).ThenByDescending(x => x.Show.Quarter).ThenBy(x => x.Show.Title)
                .Select(x => new CastRoleViewModel
                {
                    DeleteItemURL = getDeleteItemURL(x.ShowCastId),
                    ShowLinkURL = url.GetURL<ShowController>(c => c.Get(x.Show.ShowId)),
                    ShowName = ExtensionMethods.RearrangeShowTitle(x.Show.Title),
                    ShowQuarter = x.Show.Quarter,
                    ShowYear = x.Show.Year,
                    Role = x.Role,
                }));
        }

        public override Func<RelationViewModel, HelperResult> RenderItemColumns
        {
            get
            {
                return x => CastRolesTableHelper.RenderColumns((CastRoleViewModel)x);
            }
        }
    }

    public class CastRoleViewModel : RelationViewModel
    {
        public string ShowLinkURL { get; set; }
        public string ShowName { get; set; }
        public Quarter ShowQuarter { get; set; }
        public short ShowYear { get; set; }
        public string Role { get; set; }
    }
}