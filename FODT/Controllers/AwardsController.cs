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
    public partial class AwardsController : BaseController
    {
        [HttpGet, Route("year/{year?}")]
        public virtual ActionResult ByYear(short? year)
        {
            if (!year.HasValue || year > DateTime.Now.Year)
            {
                return RedirectToAction(Actions.ByYear(GetMostRecentYear()));
            }
            var showAwards = DatabaseSession.Query<ShowAward>().Where(x => x.Year == year).Fetch(x => x.Show).Fetch(x => x.Person).Fetch(x => x.Award).ToList();
            var peopleAwards = DatabaseSession.Query<PersonAward>().Where(x => x.Year == year).Fetch(x => x.Person).Fetch(x => x.Award).ToList();

            var viewModel = new ByYearViewModel();
            viewModel.Year = year.Value;
            viewModel.NextYear = (year >= DateTime.Now.Year) ? (short?)null : (short)(year.Value + 1);
            viewModel.PreviousYear = (short)(year.Value - 1);

            viewModel.Awards = showAwards.Select(x => new ByYearViewModel.Award
            {
                Year = x.Year,
                AwardId = x.Award.AwardId,
                Name = x.Award.Name,

                ShowId = x.Show.ShowId,
                ShowTitle = x.Show.Title,
                ShowQuarter = x.Show.Quarter,
                ShowYear = x.Show.Year,

                PersonId = x.Person != null ? x.Person.PersonId : (int?)null,
                PersonName = x.Person != null ? x.Person.Fullname : (string)null,
                PersonLastName = x.Person != null ? x.Person.LastName : (string)null,
            })
            .Concat(peopleAwards.Select(x => new ByYearViewModel.Award
            {
                Year = x.Year,
                AwardId = x.Award.AwardId,
                Name = x.Award.Name,

                PersonId = x.Person.PersonId,
                PersonName = x.Person.Fullname, 
                PersonLastName = x.Person.LastName,
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