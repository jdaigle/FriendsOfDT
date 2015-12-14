using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FODT.Database;
using FODT.Models;
using FODT.Models.IMDT;
using FODT.Views.Shows;
using NHibernate.Linq;

namespace FODT.Controllers
{
    [RoutePrefix("archive/shows")]
    public class ShowsController : BaseController
    {
        [HttpGet, Route("sort/year")]
        public ActionResult SortByYear()
        {
            var shows = DatabaseSession.Query<Show>().ToList();
            var viewModel = new SortedShowsViewModel();
            viewModel.Shows = shows.OrderBy(x => x).Select(x => new SortedShowsViewModel.Show
            {
                ShowLinkURL = this.GetURL<ShowController>(c => c.ShowDetails(x.ShowId)),
                ShowId = x.ShowId,
                ShowTitle = x.DisplayTitle,
                ShowQuarter = x.Quarter,
                ShowYear = x.Year,
            }).ToList();
            return View(viewModel);
        }

        [HttpGet, Route("sort/title")]
        public ActionResult SortByTitle()
        {
            var shows = DatabaseSession.Query<Show>().ToList();
            var viewModel = new SortedShowsViewModel();
            viewModel.Shows = shows.OrderBy(x => x.Title).Select(x => new SortedShowsViewModel.Show
            {
                ShowLinkURL = this.GetURL<ShowController>(c => c.ShowDetails(x.ShowId)),
                ShowId = x.ShowId,
                ShowTitle = x.DisplayTitle,
                ShowQuarter = x.Quarter,
                ShowYear = x.Year,
            }).ToList();
            return View(viewModel);
        }
    }
}