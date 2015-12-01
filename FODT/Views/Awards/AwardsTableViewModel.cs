using System;
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
    public class AwardsTableViewModel : RelationTableViewModel
    {
        public AwardsTableViewModel(UrlHelper urlHelper, Func<int, int?, string> getDeleteItemURL, IEnumerable<ShowAward> showAwards = null, IEnumerable<PersonAward> personAwards = null)
        {
            TableTitle = "Awards";
            AddItemURLText = "Add Award";

            Items = new List<RelationViewModel>();

            if (showAwards != null)
            {
                Items.AddRange(showAwards.Select(x => new AwardViewModel
                {
                    //DeleteAwardURL = this.GetURL(c => c.DeleteAward(personId, x.ShowAwardId, x.Show.ShowId)),
                    DeleteItemURL = getDeleteItemURL(x.ShowAwardId, x.Show.ShowId),
                    ShowLinkURL = urlHelper.GetURL<ShowController>(c => c.Get(x.Show.ShowId)),
                    AwardYearLinkURL = urlHelper.GetURL<AwardsController>(c => c.ByYear(x.Year)),

                    Year = x.Year,
                    Name = x.Award.Name,
                    ShowName = ExtensionMethods.RearrangeShowTitle(x.Show.Title),

                }));
            }

            if (personAwards != null)
            {
                Items.AddRange(personAwards.Select(x => new AwardViewModel
                {
                    //DeleteAwardURL = this.GetURL(c => c.DeleteAward(personId, x.PersonAwardId, null)),
                    DeleteItemURL = getDeleteItemURL(x.PersonAwardId, null),
                    AwardYearLinkURL = urlHelper.GetURL<AwardsController>(c => c.ByYear(x.Year)),

                    Year = x.Year,
                    Name = x.Award.Name,
                }));
            }

            // nasty sorting hack
            Items = Items.Cast<AwardViewModel>()
                .OrderByDescending(x => x.Year)
                .ThenBy(x => x.Name)
                .AsEnumerable<RelationViewModel>()
                .ToList();
        }

        public override Func<RelationViewModel, HelperResult> RenderItemColumns
        {
            get
            {
                return x => AwardsTableHelper.RenderColumns((AwardViewModel)x);
            }
        }
    }

    public class AwardViewModel : RelationViewModel
    {
        public string AwardYearLinkURL { get; set; }
        public string DeleteAwardURL { get; set; }
        public short Year { get; set; }
        public string Name { get; set; }
        public string ShowLinkURL { get; set; }
        public string ShowName { get; set; }
    }
}