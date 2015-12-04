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
    public class CastRolesTableViewModel : RelationTableViewModel<CastRoleViewModel>
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
                    ShowQuarter = x.Show.Quarter,
                    ShowYear = x.Show.Year,
                    ShowLinkURL = url.GetURL<ShowController>(c => c.ShowDetails(x.Show.ShowId)),
                    ShowName = x.Show.DisplayTitle,
                    PersonLinkURL = url.GetURL<PersonController>(c => c.PersonDetails(x.Person.PersonId)),
                    PersonName = x.Person.Fullname,
                    Role = x.Role,
                }));
        }
    }

    public class CastRoleViewModel : RelationViewModel
    {
        public Quarter ShowQuarter { get; set; }
        public short ShowYear { get; set; }

        public string ShowLinkURL { get; set; }
        public string ShowName { get; set; }

        public string PersonLinkURL { get; set; }
        public string PersonName { get; set; }

        public string Role { get; set; }
    }
}