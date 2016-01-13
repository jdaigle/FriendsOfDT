using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FODT.Models.IMDT;
using FODT.Views.Home;
using NHibernate.Linq;

namespace FODT.Controllers
{
    [RoutePrefix("")]
    public class HomeController : BaseController
    {
        [HttpGet, Route("")]
        public ActionResult DefaultAction()
        {
            return Redirect(Url.GetUrl(Welcome));
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

            var aMonthAgo = DateTime.UtcNow.AddMonths(-1).Date;
            var newShows = DatabaseSession.Query<Show>().Fetch(x => x.Photo).Where(x => x.InsertedDateTime >= aMonthAgo).ToList();
            var newPersons = DatabaseSession.Query<Person>().Fetch(x => x.Photo).Where(x => x.InsertedDateTime >= aMonthAgo).ToList();
            var newPhotos = DatabaseSession.Query<Photo>().Where(x => x.InsertedDateTime >= aMonthAgo).ToList();


            var viewModel = new ArchiveWelcomeViewModel();
            viewModel.ShowCount = (int)stats.Single(x => x.Key == "ShowCount").Value;
            viewModel.PersonCount = (int)stats.Single(x => x.Key == "PersonCount").Value;
            viewModel.CastCount = (int)stats.Single(x => x.Key == "CastCount").Value;
            viewModel.CrewCount = (int)stats.Single(x => x.Key == "CrewCount").Value;
            viewModel.PhotoCount = (int)stats.Single(x => x.Key == "PhotoCount").Value;

            viewModel.NewShows = newShows.OrderBy(x => x, ShowComparer.ReverseChronologicalShowComparer)
                .Select(x => new ArchiveWelcomeViewModel.NewShowViewModel
                {
                    Name = x.DisplayTitle,
                    Year = x.Year.ToString(),
                    LinkUrl = Url.For<ShowController>(c => Url.GetUrl(c.ShowDetails, x.ShowId)),
                    ImageUrl = x.Photo.GetTinyURL(),
                }).ToList();

            viewModel.NewPeople = newPersons.OrderBy(x => x.SortableName)
                .Select(x => new ArchiveWelcomeViewModel.NewPersonViewModel
                {
                    Name = x.Fullname,
                    LinkUrl = Url.For<PersonController>(c => Url.GetUrl(c.PersonDetails, x.PersonId)),
                    ImageUrl = x.Photo.GetTinyURL(),
                }).ToList();

            viewModel.NewPhotos = newPhotos.OrderByDescending(x => x.InsertedDateTime)
                .Select(x => new ArchiveWelcomeViewModel.NewPhotoViewModel
                {
                    LinkUrl = Url.GetURL<PhotosController>(c => c.GetPhotoDetail(x.PhotoId)),
                    ImageUrl = x.GetTinyURL(),
                }).ToList();

            return View(viewModel);
        }

        [HttpGet, Route("IMDT")]
        public ActionResult IMDT()
        {
            return this.RedirectToAction(c => c.ArchiveWelcome(), permanent: true);
        }

        [HttpGet, Route("Admin")]
        public ActionResult AdminHome()
        {
            return this.RedirectToAction<UserAdminController>(x => x.List());
        }
    }
}
