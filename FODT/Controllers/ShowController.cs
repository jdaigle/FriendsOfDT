﻿using System;
using System.Linq;
using System.Web.Mvc;
using FODT.Database;
using FODT.Models;
using FODT.Models.IMDT;
using FODT.Views.Shared;
using FODT.Views.Show;
using NHibernate.Linq;

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
        public ActionResult Get(int showId)
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
            var awards = DatabaseSession.Query<ShowAward>().Where(x => x.Show == show).Fetch(x => x.Person).Fetch(x => x.Award).ToList();

            var otherPerformances = DatabaseSession.Query<Show>()
                .Where(x => x.Title == show.Title &&
                            x.ShowId != show.ShowId)
                .ToList();

            var viewModel = new GetViewModel();
            viewModel.ShowId = showId;
            viewModel.Title = show.Title;
            viewModel.Author = show.Author;
            viewModel.Quarter = show.Quarter;
            viewModel.Year = show.Year;
            viewModel.FunFacts = show.FunFacts;
            viewModel.Pictures = show.Pictures;
            viewModel.Toaster = show.Toaster;
            viewModel.MediaItemId = show.MediaItem.MediaItemId;
            viewModel.PreviousShowLinkURL = previousShowId.HasValue ? this.GetURL(c => c.Get(previousShowId.Value)) : "";
            viewModel.NextShowLinkURL = nextShowId.HasValue ? this.GetURL(c => c.Get(nextShowId.Value)) : "";

            viewModel.MediaItemLinkURL = this.GetURL(c => c.GetShowMedia(showId, show.MediaItem.MediaItemId));
            viewModel.MediaItemThumbnailURL = this.GetURL<MediaController>(c => c.GetItemThumbnail(show.MediaItem.MediaItemId));
            viewModel.MediaListLinkURL = this.GetURL(c => c.ListShowMedia(showId));

            viewModel.OtherPerformances = otherPerformances.Select(x => new GetViewModel.OtherPerfViewModel {
                ShowLinkURL = this.GetURL(c => c.Get(x.ShowId)),
                Title = x.Title,
                Year = x.Year,
            }).ToList();

            viewModel.Awards = awards.Select(x => new GetViewModel.Award
            {
                AwardYearLinkURL = this.GetURL<AwardsController>(c => c.ByYear(x.Year)),
                PersonLinkURL = x.Person != null ? this.GetURL<PersonController>(c => c.PersonDetails(x.Person.PersonId)) : "",
                Year = x.Year,
                AwardId = x.ShowAwardId,
                Name = x.Award.Name,
                PersonId = x.Person != null ? x.Person.PersonId : (int?)null,
                PersonName = x.Person != null ? x.Person.Fullname : (string)null,
                PersonLastName = x.Person != null ? x.Person.LastName : (string)null,
            }).ToList();

            viewModel.Cast = cast.Select(x => new GetViewModel.CastRole
            {
                PersonLinkURL = this.GetURL<PersonController>(c => c.PersonDetails(x.Person.PersonId)),
                PersonId = x.Person.PersonId,
                PersonName = x.Person.Fullname,
                PersonLastName = x.Person.LastName,
                Role = x.Role,
            }).ToList();

            viewModel.Crew = crew.Select(x => new GetViewModel.CrewPosition
            {
                PersonLinkURL = this.GetURL<PersonController>(c => c.PersonDetails(x.Person.PersonId)),
                PersonId = x.Person.PersonId,
                PersonName = x.Person.Fullname,
                PersonLastName = x.Person.LastName,
                Name = x.Position,
                DisplayOrder = x.DisplayOrder,
            }).ToList();

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
            viewModel.ShowLinkURL = this.GetURL(c => c.Get(showId));
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
            viewModel.ShowLinkURL = this.GetURL(c => c.Get(showId));
            viewModel.ShowTitle = show.Title;
            viewModel.ShowYear = show.Year;
            viewModel.PreviousItemLinkURL = previousId.HasValue ? this.GetURL(c => c.GetShowMedia(showId, previousId.Value)) : "";
            viewModel.NextItemLinkURL = nextId.HasValue ? this.GetURL(c => c.GetShowMedia(showId, nextId.Value)) : "";
            viewModel.MediaItemId = media.MediaItem.MediaItemId;
            viewModel.MediaItemViewModel = new MediaItemViewModel();
            viewModel.MediaItemViewModel.PopulateFromDatabase(DatabaseSession, Url, mediaItemId);

            return View(viewModel);
        }
    }
}