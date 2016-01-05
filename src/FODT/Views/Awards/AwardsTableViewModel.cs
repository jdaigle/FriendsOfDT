﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;
using FODT.Controllers;
using FODT.Models;
using FODT.Models.IMDT;
using FODT.Views.Shared;

namespace FODT.Views.Awards
{
    public class AwardsTableViewModel : EditableListTableViewModel<AwardViewModel>
    {
        public AwardsTableViewModel(UrlHelper urlHelper, Func<int, string> getDeleteItemURL, IEnumerable<Award> awards)
        {
            TableTitle = "Awards";
            AddItemURLText = "Add Award";

            Items = new List<EditableListViewModel>();

            if (awards != null)
            {
                Items.AddRange(awards.Select(x => new AwardViewModel
                {
                    DeleteItemURL = getDeleteItemURL(x.AwardId),
                    AwardYearLinkURL = urlHelper.GetURL<AwardsController>(c => c.ByYear(x.Year)),

                    Year = x.Year,
                    Name = x.AwardType.Name,

                    ShowLinkURL = x.Show != null ? urlHelper.GetURL<ShowController>(c => c.ShowDetails(x.Show.ShowId)) : "",
                    ShowName = x.Show != null ? x.Show.DisplayTitle : "",

                    PersonLinkURL = x.Person != null ? urlHelper.GetURL<PersonController>(c => c.PersonDetails(x.Person.PersonId)) : "",
                    PersonName = x.Person != null ? x.Person.Fullname : "",
                }));
            }

            // nasty sorting hack
            Items = Items.Cast<AwardViewModel>()
                .OrderByDescending(x => x.Year)
                .ThenBy(x => x.Name)
                .AsEnumerable<EditableListViewModel>()
                .ToList();
        }
    }

    public class AwardViewModel : EditableListViewModel
    {
        public string AwardYearLinkURL { get; set; }
        public string DeleteAwardURL { get; set; }
        public short Year { get; set; }
        public string Name { get; set; }

        public string ShowLinkURL { get; set; }
        public string ShowName { get; set; }

        public string PersonLinkURL { get; set; }
        public string PersonName { get; set; }

        public bool ToPerson { get { return !PersonLinkURL.IsNullOrWhiteSpace(); } }
        public bool ForShow { get { return !ShowLinkURL.IsNullOrWhiteSpace(); } }
    }
}