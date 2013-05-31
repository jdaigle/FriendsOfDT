using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AttributeRouting;
using AttributeRouting.Web.Mvc;
using FODT.Database;
using FODT.Models;
using FODT.Models.IMDT;
using FODT.Views.Shows;
using NHibernate.Linq;

namespace FODT.Controllers
{
    [RoutePrefix("shows")]
    public partial class ShowsController : BaseController
    {
        public class ShowOrderDto
        {
            public virtual int ShowId { get; set; }
            public virtual string Title { get; set; }
            public virtual Quarter Quarter { get; set; }
            public virtual short Year { get; set; }
        }

        [GET("sort/year")]
        public virtual ActionResult SortByYear()
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
                ShowId = x.ShowId,
                ShowTitle = x.Title,
                ShowQuarter = x.Quarter,
                ShowYear = x.Year,
            }).ToList();
            return View(viewModel);
        }

        [GET("sort/title")]
        public virtual ActionResult SortByTitle()
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
                ShowId = x.ShowId,
                ShowTitle = x.Title,
                ShowQuarter = x.Quarter,
                ShowYear = x.Year,
            }).ToList();
            return View(viewModel);
        }
    }
}