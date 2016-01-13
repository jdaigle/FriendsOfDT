using System;
using System.Linq;
using System.Web.Mvc;
using FODT.Database;
using FODT.Models;
using FODT.Models.IMDT;
using FODT.Views.Awards;
using FODT.Views.Show;
using NHibernate.Linq;
using FODT.Security;
using FODT.Views.Photos;

namespace FODT.Controllers
{
    [RoutePrefix("archive/Show")]
    public class ShowController : BaseController
    {
        [HttpGet, Route("{showId}")]
        public ActionResult ShowDetails(int showId)
        {
            var orderedShows = DatabaseSession.Query<Show>().ToList().OrderBy(x => x).ToList();
            var index = orderedShows.IndexOf(orderedShows.Single(x => x.ShowId == showId));
            var previousShowId = index > 0 ? orderedShows[index - 1].ShowId : (int?)null;
            var nextShowId = index < orderedShows.Count - 1 ? orderedShows[index + 1].ShowId : (int?)null;

            var show = DatabaseSession.Get<Show>(showId);

            var crew = DatabaseSession.Query<ShowCrew>().Where(x => x.Show == show).Fetch(x => x.Person).ToList();
            var cast = DatabaseSession.Query<ShowCast>().Where(x => x.Show == show).Fetch(x => x.Person).ToList();
            var awards = DatabaseSession.Query<Award>().Where(x => x.Show == show).Fetch(x => x.Person).Fetch(x => x.AwardType).ToList();

            var relatedPhotos = DatabaseSession.Query<ShowPhoto>().Where(x => x.Show == show).Fetch(x => x.Photo).ToList();

            var otherPerformances = DatabaseSession.Query<Show>()
                .Where(x => x.Title == show.Title &&
                            x.ShowId != show.ShowId)
                .ToList();

            var viewModel = new ShowDetailsViewModel();

            viewModel.PhotoUploadLinkURL = this.GetURL<PhotosController>(c => c.Upload());
            viewModel.PhotoLinkURL = this.GetURL<ShowPhotosController>(c => c.ListShowPhotos(showId, show.Photo.PhotoId));
            viewModel.PhotoThumbnailURL = show.Photo.GetThumbnailURL();
            viewModel.PhotoListLinkURL = this.GetURL<ShowPhotosController>(c => c.ListShowPhotos(showId, null));

            viewModel.Title = show.DisplayTitle;
            viewModel.Author = show.Author;
            viewModel.Quarter = show.Quarter;
            viewModel.Year = show.Year;
            viewModel.FunFacts = show.FunFacts;
            viewModel.Pictures = show.Pictures;
            viewModel.Toaster = show.Toaster;
            viewModel.PreviousShowLinkURL = previousShowId.HasValue ? this.GetURL(c => c.ShowDetails(previousShowId.Value)) : "";
            viewModel.NextShowLinkURL = nextShowId.HasValue ? this.GetURL(c => c.ShowDetails(nextShowId.Value)) : "";

            viewModel.OtherPerformances = new OtherPerformancesTableViewModel(this.Url, otherPerformances);

            viewModel.AwardsTable = new AwardsTableViewModel(
                this.Url
                , id => this.GetURL(c => c.DeleteAward(showId, id))
                , awards)
            {
                CanEdit = this.ControllerContext.CanEditShow(show),
                AddItemURL = this.GetURL(c => c.AddAward(showId)),
            };

            viewModel.CastRolesTable = new CastRolesTableViewModel(
                this.Url
                , id => this.GetURL(c => c.DeleteCast(showId, id))
                , cast)
            {
                CanEdit = this.ControllerContext.CanEditShow(show),
                AddItemURL = this.GetURL(c => c.AddCast(showId)),
            };

            viewModel.CrewPositionsTable = new CrewPositionsTableViewModel(
                this.Url
                , id => this.GetURL(c => c.DeleteCrew(showId, id))
                , crew)
            {
                CanEdit = this.ControllerContext.CanEditShow(show),
                AddItemURL = this.GetURL(c => c.AddCrew(showId)),
            };

            viewModel.PhotoCount = relatedPhotos.Count;
            viewModel.NewPhotos = relatedPhotos
                .OrderByDescending(x => x.InsertedDateTime)
                .Where(x => x.Photo.PhotoId != show.Photo.PhotoId)
                .Select(x => new ShowDetailsViewModel.NewPhotoViewModel
                {
                    PhotoLinkURL = this.GetURL<ShowPhotosController>(c => c.ListShowPhotos(showId, x.Photo.PhotoId)),
                    PhotoTinyURL = x.Photo.GetTinyURL(),
                })
                .Take(4)
                .ToList();

            return View(viewModel);
        }

        [HttpGet, Route("{showId}/AddAward")]
        public ActionResult AddAward(int showId)
        {
            if (!Request.IsAjaxRequest())
            {
                return this.RedirectToAction(x => x.ShowDetails(showId));
            }

            throw new NotImplementedException();
        }

        [HttpPost, Route("{showId}/AddAward")]
        public ActionResult AddAward(int showId, int awardId, short year, int? personId)
        {
            Person person = null;
            if (personId.HasValue)
            {
                person = DatabaseSession.Load<Person>(personId.Value);
            }
            var award = new Award(DatabaseSession.Load<Show>(showId), person, DatabaseSession.Load<AwardType>(awardId), year);
            DatabaseSession.Save(award);
            DatabaseSession.CommitTransaction();
            return this.RedirectToAction(x => x.ShowDetails(showId));
        }

        [HttpPost, Route("{showId}/DeleteAward")]
        public ActionResult DeleteAward(int showId, int awardId)
        {
            var award = DatabaseSession.Get<Award>(awardId);
            DatabaseSession.Delete(award);
            DatabaseSession.CommitTransaction();
            return this.RedirectToAction(x => x.ShowDetails(showId));
        }

        [HttpGet, Route("{showId}/AddCast")]
        public ActionResult AddCast(int showId)
        {
            if (!Request.IsAjaxRequest())
            {
                return this.RedirectToAction(x => x.ShowDetails(showId));
            }

            throw new NotImplementedException();
        }

        [HttpPost, Route("{showId}/AddCast")]
        public ActionResult AddCast(int showId, int personId, string role)
        {
            var entity = new ShowCast(DatabaseSession.Load<Person>(personId), DatabaseSession.Load<Show>(showId), role);
            DatabaseSession.Save(entity);
            DatabaseSession.CommitTransaction();
            return this.RedirectToAction(x => x.ShowDetails(showId));
        }

        [HttpPost, Route("{showId}/DeleteCast")]
        public ActionResult DeleteCast(int showId, int showCastId)
        {
            var entity = DatabaseSession.Get<ShowCast>(showCastId);
            DatabaseSession.Delete(entity);
            DatabaseSession.CommitTransaction();
            return this.RedirectToAction(x => x.ShowDetails(showId));
        }

        [HttpGet, Route("{showId}/AddCrew")]
        public ActionResult AddCrew(int showId)
        {
            if (!Request.IsAjaxRequest())
            {
                return this.RedirectToAction(x => x.ShowDetails(showId));
            }

            throw new NotImplementedException();
        }

        [HttpPost, Route("{showId}/AddCrew")]
        public ActionResult AddCrew(int showId, int personId, string position)
        {
            var entity = new ShowCrew(DatabaseSession.Load<Person>(personId), DatabaseSession.Load<Show>(showId), position);
            DatabaseSession.Save(entity);
            DatabaseSession.CommitTransaction();
            return this.RedirectToAction(x => x.ShowDetails(showId));
        }

        [HttpPost, Route("{showId}/DeleteCrew")]
        public ActionResult DeleteCrew(int showId, int showCrewId)
        {
            var entity = DatabaseSession.Get<ShowCrew>(showCrewId);
            DatabaseSession.Delete(entity);
            DatabaseSession.CommitTransaction();
            return this.RedirectToAction(x => x.ShowDetails(showId));
        }
    }
}