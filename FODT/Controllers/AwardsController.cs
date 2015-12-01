using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NHibernate.Linq;
using FODT.Models.IMDT;
using FODT.Views.Awards;

namespace FODT.Controllers
{
    [RoutePrefix("awards")]
    public class AwardsController : BaseController
    {
        [HttpGet, Route("year/{year?}")]
        public ActionResult ByYear(short? year)
        {
            if (!year.HasValue || year > DateTime.Now.Year)
            {
                return this.RedirectToAction(c => c.ByYear(GetMostRecentYear()));
            }
            var showAwards = DatabaseSession.Query<ShowAward>().Where(x => x.Year == year).Fetch(x => x.Show).Fetch(x => x.Person).Fetch(x => x.Award).ToList();
            var peopleAwards = DatabaseSession.Query<PersonAward>().Where(x => x.Year == year).Fetch(x => x.Person).Fetch(x => x.Award).ToList();

            var viewModel = new ByYearViewModel();
            viewModel.Year = year.Value;
            var nextYear = (short)(year.Value + 1);
            var prevYear = (short)(year.Value - 1);
            viewModel.NextYearURL = this.GetURL(c => c.ByYear(nextYear));
            viewModel.PreviousYearURL = this.GetURL(c => c.ByYear(prevYear));

            viewModel.Awards = showAwards.Select(x => new ByYearViewModel.Award
            {
                Year = x.Year,
                AwardId = x.Award.AwardId,
                Name = x.Award.Name,

                AwardShowLinkURL = this.GetURL<ShowController>(c => c.ShowDetails(x.Show.ShowId)),
                AwardShowLinkText = x.Show.Title,

                AwardPersonLinkURL = x.Person != null ? this.GetURL<PersonController>(c => c.PersonDetails(x.Person.PersonId)) : "",
                AwardPersonLinkText = x.Person != null ? x.Person.Fullname : "",
            })
            .Concat(peopleAwards.Select(x => new ByYearViewModel.Award
            {
                Year = x.Year,
                AwardId = x.Award.AwardId,
                Name = x.Award.Name,

                AwardShowLinkURL = "",
                AwardShowLinkText = "",

                AwardPersonLinkURL = this.GetURL<PersonController>(c => c.PersonDetails(x.Person.PersonId)),
                AwardPersonLinkText = x.Person.Fullname,
            })).OrderBy(x => x.AwardId).ToList();

            return View(viewModel);
        }

        private short GetMostRecentYear()
        {
            var mostRecentShowYear = DatabaseSession.Query<ShowAward>().OrderByDescending(x => x.Year).Take(1).ToList().Select(x => x.Year).Single();
            var mostRecentPersonYear = DatabaseSession.Query<PersonAward>().OrderByDescending(x => x.Year).Take(1).ToList().Select(x => x.Year).Single();
            return mostRecentShowYear > mostRecentPersonYear ? mostRecentShowYear : mostRecentPersonYear;
        }
    }
}