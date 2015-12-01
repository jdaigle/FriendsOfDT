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
    public class CrewPositionsTableViewModel : RelationTableViewModel
    {
        public CrewPositionsTableViewModel(UrlHelper url, Func<int, string> getDeleteItemURL, List<ShowCrew> crew)
        {
            TableTitle = "Crew Credits";
            AddItemURLText = "Add Crew Credit";

            Items = new List<RelationViewModel>();
            Items.AddRange(crew
                .OrderByDescending(x => x.Show.Year).ThenByDescending(x => x.Show.Quarter).ThenBy(x => x.Show.Title)
                .Select(x => new CrewPositionViewModel
                {
                    DeleteItemURL = getDeleteItemURL(x.ShowCrewId),
                    ShowLinkURL = url.GetURL<ShowController>(c => c.Get(x.Show.ShowId)),
                    ShowName = ExtensionMethods.RearrangeShowTitle(x.Show.Title),
                    ShowQuarter = x.Show.Quarter,
                    ShowYear = x.Show.Year,
                    Name = x.Position,
                }));
        }

        public override Func<RelationViewModel, HelperResult> RenderItemColumns
        {
            get
            {
                return x => CrewPositionsTableHelper.RenderColumns((CrewPositionViewModel)x);
            }
        }
    }

    public class CrewPositionViewModel : RelationViewModel
    {
        public string ShowLinkURL { get; set; }
        public string ShowName { get; set; }
        public Quarter ShowQuarter { get; set; }
        public short ShowYear { get; set; }
        public string Name { get; set; }
    }
}