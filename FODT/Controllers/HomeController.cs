using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FODT.Views.Home;

namespace FODT.Controllers
{
    [RoutePrefix("")]
    public partial class HomeController : BaseController
    {
        [HttpGet, Route("")]
        public virtual ActionResult Welcome()
        {
            return View();
        }

        [HttpGet, Route("Index")]
        public virtual ActionResult Index()
        {
            return View();
        }

        [HttpGet, Route("Admin")]
        public virtual ActionResult Admin()
        {
            var viewModel = new AdminViewModel();
            viewModel.PopulateFromDatabase(DatabaseSession);
            return View(viewModel);
        }

        [HttpGet, Route("Admin/EditPerson")]
        public virtual ActionResult Admin_EditPerson(int personId)
        {
            return RedirectToAction(MVC.Person.Edit(personId));
        }
        [HttpGet, Route("Admin/ShowPerson")]
        public virtual ActionResult Admin_ShowPerson(int showId)
        {
            //return RedirectToAction(MVC.Person.Edit(personId));
            throw new NotImplementedException();
        }
    }
}
