using System;
using System.Linq;
using System.Web.Mvc;
using FODT.Database;
using FODT.Models;
using FODT.Models.IMDT;
using FODT.Views.Awards;
using FODT.Views.Shared;
using FODT.Views.Show;
using NHibernate.Linq;
using FODT.Security;

namespace FODT.Controllers
{
    [RoutePrefix("Show")]
    public class ShowController : BaseController
    {
        public class ShowOrderDto
        {
            public int ShowId { get; set; }
            public string Title { get; set; }
            public Quarter Quarter { get; set; }
            public short Year { get; set; }
        }

        [HttpGet, Route("{showId}")]
        public ActionResult ShowDetails(int showId)
        {
            var orderedShows = DatabaseSession.Query<Show>().Select(x => new ShowOrderDto()
                {
                    ShowId = x.ShowId,
                    Quarter = x.Quarter,
                    Title = x.Title,
                    Year = x.Year,
                }).ToList().OrderBy(x => x.Year).ThenBy(x => x.Quarter).ThenBy(x => x.Title).ToList();
            var index = orderedShows.IndexOf(orderedShows.Single(x => x.ShowId == showId));
            var previousShowId = index > 0 ? orderedShows[index - 1].ShowId : (int?)null;
            var nextShowId = index < orderedShows.Count - 1 ? orderedShows[index + 1].ShowId : (int?)null;

            var show = DatabaseSession.Get<Show>(showId);

            var crew = DatabaseSession.Query<ShowCrew>().Where(x => x.Show == show).Fetch(x => x.Person).ToList();
            var cast = DatabaseSession.Query<ShowCast>().Where(x => x.Show == show).Fetch(x => x.Person).ToList();
            var awards = DatabaseSession.Query<Award>().Where(x => x.Show == show).Fetch(x => x.Person).Fetch(x => x.AwardType).ToList();

            var relatedMedia = DatabaseSession.Query<ShowMedia>().Where(x => x.Show == show).Fetch(x => x.MediaItem).ToList();

            var otherPerformances = DatabaseSession.Query<Show>()
                .Where(x => x.Title == show.Title &&
                            x.ShowId != show.ShowId)
                .ToList();

            var viewModel = new ShowDetailsViewModel();

            viewModel.MediaUploadLinkURL = this.GetURL<MediaController>(c => c.Upload());
            viewModel.MediaLinkURL = this.GetURL(c => c.GetShowMedia(showId, show.MediaItem.MediaItemId));
            viewModel.MediaThumbnailURL = this.GetURL<MediaController>(c => c.GetItemThumbnail(show.MediaItem.MediaItemId));
            viewModel.MediaListLinkURL = this.GetURL(c => c.ListShowMedia(showId));

            viewModel.Title = show.Title;
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

            viewModel.RelatedMediaCount = relatedMedia.Count;
            viewModel.NewRelatedMedia = relatedMedia
                .OrderByDescending(x => x.InsertedDateTime)
                .Where(x => x.MediaItem.MediaItemId != show.MediaItem.MediaItemId)
                .Select(x => new ShowDetailsViewModel.RelatedMediaViewModel
                {
                    ID = x.MediaItem.MediaItemId,
                    MediaLinkURL = this.GetURL(c => c.GetShowMedia(showId, x.MediaItem.MediaItemId)),
                    MediaThumbnailURL = this.GetURL<MediaController>(c => c.GetItemTiny(x.MediaItem.MediaItemId)),
                })
                .Take(4)
                .ToList();

            return View(viewModel);
        }

        [HttpGet, Route("{showId}/Media")]
        public ActionResult ListShowMedia(int showId)
        {
            var show = DatabaseSession.Get<Show>(showId);
            var relatedMedia = DatabaseSession.Query<ShowMedia>().Where(x => x.Show == show).Fetch(x => x.MediaItem).ToList();

            var viewModel = new ListShowMediaViewModel();
            viewModel.ShowTitle = show.Title;
            viewModel.ShowYear = show.Year;
            viewModel.ShowLinkURL = this.GetURL(c => c.ShowDetails(showId));
            viewModel.RelatedMedia = relatedMedia.OrderBy(x => x.MediaItem.InsertedDateTime).ThenBy(x => x.MediaItem.MediaItemId).Select(x => new ListShowMediaViewModel.Media
            {
                MediaTinyURL = this.GetURL<MediaController>(c => c.GetItemTiny(x.MediaItem.MediaItemId)),
                MediaThumbnailURL = this.GetURL<MediaController>(c => c.GetItemThumbnail(x.MediaItem.MediaItemId)),
            }).ToList();
            return View(viewModel);
        }

        [HttpGet, Route("{showId}/Media/{mediaItemId}")]
        public ActionResult GetShowMedia(int showId, int mediaItemId)
        {
            var show = DatabaseSession.Get<Show>(showId);
            var relatedMedia = DatabaseSession
                .Query<ShowMedia>().Where(x => x.Show == show).Fetch(x => x.MediaItem)
                .ToList()
                .OrderBy(x => x.MediaItem.InsertedDateTime).ThenBy(x => x.MediaItem.MediaItemId)
                .ToList();
            var media = relatedMedia.SingleOrDefault(x => x.MediaItem.MediaItemId == mediaItemId);

            var index = relatedMedia.IndexOf(relatedMedia.Single(x => x.ShowMediaId == media.ShowMediaId));
            var previousId = index > 0 ? relatedMedia[index - 1].MediaItem.MediaItemId : (int?)null;
            var nextId = index < relatedMedia.Count - 1 ? relatedMedia[index + 1].MediaItem.MediaItemId : (int?)null;

            var viewModel = new GetShowMediaViewModel();
            viewModel.UploadLinkURL = this.GetURL<MediaController>(c => c.Upload());
            viewModel.ShowLinkURL = this.GetURL(c => c.ShowDetails(showId));
            viewModel.ShowTitle = show.Title;
            viewModel.ShowYear = show.Year;
            viewModel.PreviousItemLinkURL = previousId.HasValue ? this.GetURL(c => c.GetShowMedia(showId, previousId.Value)) : "";
            viewModel.NextItemLinkURL = nextId.HasValue ? this.GetURL(c => c.GetShowMedia(showId, nextId.Value)) : "";
            viewModel.MediaItemId = media.MediaItem.MediaItemId;
            viewModel.MediaItemViewModel = new MediaItemViewModel();
            viewModel.MediaItemViewModel.PopulateFromDatabase(DatabaseSession, Url, mediaItemId);

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
        public ActionResult DeleteAward(int showId, int showAwardId)
        {
            var award = DatabaseSession.Get<Award>(showAwardId);
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