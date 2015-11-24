using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FODT.Views.Home;

namespace FODT.Controllers
{
    [RoutePrefix("")]
    public class HomeController : BaseController
    {
        [HttpGet, Route("")]
        public ActionResult Welcome()
        {
            return View();
        }

        [HttpGet, Route("Index")]
        public ActionResult Index()
        {
            return View();
        }

        //[HttpGet, Route("Admin")]
        //public ActionResult Admin()
        //{
        //    var viewModel = new AdminViewModel();
        //    viewModel.PopulateFromDatabase(DatabaseSession);
        //    return View(viewModel);
        //}

        [HttpGet, Route("Admin/EditPerson")]
        public ActionResult Admin_EditPerson(int personId)
        {
            return this.RedirectToAction<PersonController>(x => x.EditPerson(personId));
        }
        [HttpGet, Route("Admin/ShowPerson")]
        public ActionResult Admin_ShowPerson(int showId)
        {
            //return RedirectToAction(MVC.Person.Edit(personId));
            throw new NotImplementedException();
        }
    }
}
