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
    [RoutePrefix("archive/awards")]
    public class AwardsController : BaseController
    {
        [HttpGet, Route("")]
        public ActionResult Index()
        {
            return this.RedirectToAction(c => c.ByYear(GetMostRecentYear()));
        }

        [HttpGet, Route("year/{year?}")]
        public ActionResult ByYear(short? year)
        {
            if (!year.HasValue || year > DateTime.Now.Year)
            {
                return this.RedirectToAction(c => c.ByYear(GetMostRecentYear()));
            }
            var awards = DatabaseSession.Query<Award>().Where(x => x.Year == year).Fetch(x => x.Show).Fetch(x => x.Person).Fetch(x => x.AwardType).ToList();

            var viewModel = new ByYearViewModel();
            viewModel.Year = year.Value;
            var nextYear = (short)(year.Value + 1);
            var prevYear = (short)(year.Value - 1);
            viewModel.NextYearURL = this.GetURL(c => c.ByYear(nextYear));
            viewModel.PreviousYearURL = this.GetURL(c => c.ByYear(prevYear));

            viewModel.AwardsTable = new AwardsTableViewModel(
                this.Url
                , id => ""
                , awards)
            {
                CanEdit = false,
                //AddItemURL = this.GetURL(c => c.AddAward(personId)),
            };

            return View(viewModel);
        }

        private short GetMostRecentYear()
        {
            return DatabaseSession.Query<Award>().OrderByDescending(x => x.Year).Take(1).ToList().Select(x => x.Year).Single();
        }

        [HttpPost, Route("DeleteAward")]
        public ActionResult DeleteAward(int awardId)
        {
            var award = DatabaseSession.Get<Award>(awardId);
            DatabaseSession.Delete(award);
            return this.RedirectToAction(x => x.ByYear(award.Year));
        }
    }
}