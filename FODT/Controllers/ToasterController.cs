using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AttributeRouting;
using AttributeRouting.Web.Mvc;
using FODT.Models;
using FODT.Models.IMDT;
using FODT.Views.Toaster;
using NHibernate.Linq;

namespace FODT.Controllers
{
    [RoutePrefix("Toaster")]
    public partial class ToasterController : BaseController
    {
        [GET("Hunt")]
        public virtual ActionResult Hunt()
        {
            var shows = DatabaseSession.Query<Show>().Where(x => x.Toaster != string.Empty).ToList();
            var viewModel = new HuntViewModel();
            viewModel.Shows.AddRange(shows.Select(x => new HuntViewModel.Show
            {
                ShowId = x.ShowId,
                ShowName = x.Title,
                ShowQuarter = x.Quarter,
                ShowYear = x.Year,
                Toaster = x.Toaster,
            }));
            return View(viewModel);
        }
    }
}