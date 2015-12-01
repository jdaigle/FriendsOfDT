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
    [RoutePrefix("shows")]
    public class ShowsController : BaseController
    {
        public class ShowOrderDto
        {
            public int ShowId { get; set; }
            public string Title { get; set; }
            public Quarter Quarter { get; set; }
            public short Year { get; set; }
        }

        [HttpGet, Route("sort/year")]
        public ActionResult SortByYear()
        {
            var shows = DatabaseSession.Query<Show>().Select(x => new ShowOrderDto()
            {
                ShowId = x.ShowId,
                Quarter = x.Quarter,
                Title = x.Title,
                Year = x.Year,
            }).ToList();
            var viewModel = new SortedShowsViewModel();
            viewModel.Shows = shows.OrderBy(x => x.Year).ThenBy(x => x.Quarter).ThenBy(x => x.Title).Select(x => new SortedShowsViewModel.Show
            {
                ShowLinkURL = this.GetURL<ShowController>(c => c.ShowDetails(x.ShowId)),
                ShowId = x.ShowId,
                ShowTitle = x.Title,
                ShowQuarter = x.Quarter,
                ShowYear = x.Year,
            }).ToList();
            return View(viewModel);
        }

        [HttpGet, Route("sort/title")]
        public ActionResult SortByTitle()
        {
            var shows = DatabaseSession.Query<Show>().Select(x => new ShowOrderDto()
            {
                ShowId = x.ShowId,
                Quarter = x.Quarter,
                Title = x.Title,
                Year = x.Year,
            }).ToList();
            var viewModel = new SortedShowsViewModel();
            viewModel.Shows = shows.OrderBy(x => x.Title).Select(x => new SortedShowsViewModel.Show
            {
                ShowLinkURL = this.GetURL<ShowController>(c => c.ShowDetails(x.ShowId)),
                ShowId = x.ShowId,
                ShowTitle = x.Title,
                ShowQuarter = x.Quarter,
                ShowYear = x.Year,
            }).ToList();
            return View(viewModel);
        }
    }
}