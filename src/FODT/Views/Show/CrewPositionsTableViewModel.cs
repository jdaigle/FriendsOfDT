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
    public class CrewPositionsTableViewModel : RelationTableViewModel<CrewPositionViewModel>
    {
        public CrewPositionsTableViewModel(UrlHelper url, Func<int, string> getDeleteItemURL, List<ShowCrew> crew)
        {
            TableTitle = "Crew Credits";
            AddItemURLText = "Add Crew Credit";

            Items = new List<RelationViewModel>();
            Items.AddRange(crew
                .OrderBy(x => x.Show, Models.IMDT.Show.ReverseChronologicalShowComparer)
                .ThenBy(x => x.DisplayOrder)
                .ThenBy(x => x.Position)
                .Select(x => new CrewPositionViewModel
                {
                    DeleteItemURL = getDeleteItemURL(x.ShowCrewId),
                    ShowQuarter = x.Show.Quarter,
                    ShowYear = x.Show.Year,
                    ShowLinkURL = url.GetURL<ShowController>(c => c.ShowDetails(x.Show.ShowId)),
                    ShowName = x.Show.DisplayTitle,
                    PersonLinkURL = url.GetURL<PersonController>(c => c.PersonDetails(x.Person.PersonId)),
                    PersonName = x.Person.Fullname,
                    Name = x.Position,
                }));
        }
    }

    public class CrewPositionViewModel : RelationViewModel
    {
        public Quarter ShowQuarter { get; set; }
        public short ShowYear { get; set; }

        public string ShowLinkURL { get; set; }
        public string ShowName { get; set; }

        public string PersonLinkURL { get; set; }
        public string PersonName { get; set; }

        public string Name { get; set; }
    }
}