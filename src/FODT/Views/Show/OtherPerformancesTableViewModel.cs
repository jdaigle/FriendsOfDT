using System;
using System.Linq;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.WebPages;
using FODT.Models.IMDT;
using FODT.Views.Shared;
using FODT.Controllers;
using FODT.Models;

namespace FODT.Views.Show
{
    public class OtherPerformancesTableViewModel : RelationTableViewModel<OtherPerformancesViewModel>
    {
        public OtherPerformancesTableViewModel(UrlHelper url, List<FODT.Models.IMDT.Show> otherPerformances)
        {
            TableTitle = "Other Performances";

            Items = new List<RelationViewModel>();
            Items.AddRange(otherPerformances.OrderBy(x => x.Year)
                .Select(x => new OtherPerformancesViewModel
                {
                    ShowQuarter = x.Quarter,
                    ShowYear = x.Year,
                    ShowLinkURL = url.GetURL<ShowController>(c => c.ShowDetails(x.ShowId)),
                    ShowName = ExtensionMethods.RearrangeShowTitle(x.Title),
                }));
        }
    }

    public class OtherPerformancesViewModel : RelationViewModel
    {
        public Quarter ShowQuarter { get; set; }
        public short ShowYear { get; set; }

        public string ShowLinkURL { get; set; }
        public string ShowName { get; set; }
    }
}