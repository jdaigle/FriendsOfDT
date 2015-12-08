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
        public ActionResult Index()
        {
            return this.RedirectToAction(c => c.Welcome());
        }

        [HttpGet, Route("Welcome")]
        public ActionResult Welcome()
        {
            return View();
        }

        [HttpGet, Route("Archive")]
        public ActionResult ArchiveWelcome()
        {
            var stats = DatabaseSession.Query<dynamic>(@"
      SELECT 'ShowCount'   AS [Key], COUNT(*) AS [Value] FROM Show
UNION SELECT 'PersonCount' AS [Key], COUNT(*) AS [Value] FROM Person
UNION SELECT 'CastCount'   AS [Key], COUNT(*) AS [Value] FROM ShowCast
UNION SELECT 'CrewCount'   AS [Key], COUNT(*) AS [Value] FROM ShowCrew
UNION SELECT 'PhotoCount'  AS [Key], COUNT(*) AS [Value] FROM Photo
                ");
            var viewModel = new ArchiveWelcomeViewModel();
            viewModel.ShowCount = (int)stats.Single(x => x.Key == "ShowCount").Value;
            viewModel.PersonCount = (int)stats.Single(x => x.Key == "PersonCount").Value;
            viewModel.CastCount = (int)stats.Single(x => x.Key == "CastCount").Value;
            viewModel.CrewCount = (int)stats.Single(x => x.Key == "CrewCount").Value;
            viewModel.PhotoCount = (int)stats.Single(x => x.Key == "PhotoCount").Value;
            return View(viewModel);
        }

        [HttpGet, Route("IMDT")]
        public ActionResult IMDT()
        {
            return this.RedirectToAction(c => c.ArchiveWelcome(), permanent: true);
        }

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
