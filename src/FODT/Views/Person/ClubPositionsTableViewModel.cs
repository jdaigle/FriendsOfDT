using System;
using System.Linq;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.WebPages;
using FODT.Models.IMDT;
using FODT.Views.Shared;

namespace FODT.Views.Person
{
    public class ClubPositionsTableViewModel : EditableListTableViewModel<ClubPositionViewModel>
    {
        public ClubPositionsTableViewModel(UrlHelper url, Func<int, string> getDeleteItemURL, List<PersonClubPosition> clubPositions)
        {
            TableTitle = "EC and Club Responsibilities";
            AddItemURLText = "Add Position";

            Items = new List<EditableListViewModel>();
            Items.AddRange(clubPositions.OrderBy(x => x.DisplayOrder).ThenByDescending(x => x.Year).Select(x => new ClubPositionViewModel
            {
                DeleteItemURL = getDeleteItemURL(x.PersonClubPositionId),
                Year = x.Year,
                Name = x.Position,
            }));
        }
    }

    public class ClubPositionViewModel : EditableListViewModel
    {
        public short Year { get; set; }
        public string Name { get; set; }
    }
}