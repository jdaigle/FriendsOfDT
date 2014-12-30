using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using FODT.Database;
using FODT.Models.IMDT;
using FODT.Views.Person;
using FODT.Views.Shared;
using NHibernate.Linq;

namespace FODT.Controllers
{
    [RoutePrefix("Person")]
    public partial class PersonController : BaseController
    {
        [HttpGet, Route("{personId}")]
        public virtual ActionResult Get(int personId)
        {
            var person = DatabaseSession.Get<Person>(personId);
            var clubPositions = DatabaseSession.Query<PersonClubPosition>().Where(x => x.Person == person).ToList();
            var crew = DatabaseSession.Query<ShowCrew>().Where(x => x.Person == person).Fetch(x => x.Show).ToList();
            var cast = DatabaseSession.Query<ShowCast>().Where(x => x.Person == person).Fetch(x => x.Show).ToList();
            var showAwards = DatabaseSession.Query<ShowAward>().Where(x => x.Person == person).Fetch(x => x.Show).Fetch(x => x.Award).ToList();
            var myAwards = DatabaseSession.Query<PersonAward>().Where(x => x.Person == person).Fetch(x => x.Award).ToList();
            var relatedMedia = DatabaseSession.Query<PersonMedia>().Where(x => x.Person == person).Fetch(x => x.MediaItem).ToList();

            var viewModel = new GetViewModel();
            viewModel.PersonId = personId;
            viewModel.FullName = person.Fullname;
            viewModel.Biography = person.Biography;
            viewModel.MediaItemId = person.MediaItem.MediaItemId;
            viewModel.ClubPositions = clubPositions.OrderBy(x => x.DisplayOrder).ThenByDescending(x => x.Year).Select(x => new GetViewModel.ClubPosition
            {
                Year = x.Year,
                Name = x.Position,
                ClubPositionId = x.PersonClubPositionId,
            }).ToList();
            viewModel.Awards = showAwards.Select(x => new GetViewModel.Award
            {
                Year = x.Year,
                AwardId = x.ShowAwardId,
                Name = x.Award.Name,
                ShowId = x.Show.ShowId,
                ShowName = x.Show.Title,
                ShowQuarter = x.Show.Quarter,
                ShowYear = x.Show.Year,
            })
            .Concat(myAwards.Select(x => new GetViewModel.Award
            {
                Year = x.Year,
                AwardId = x.PersonAwardId,
                Name = x.Award.Name,
            })).ToList();
            viewModel.CastRoles = cast.Select(x => new GetViewModel.CastRole
            {
                ShowCastId = x.ShowCastId,
                ShowId = x.Show.ShowId,
                ShowName = x.Show.Title,
                ShowQuarter = x.Show.Quarter,
                ShowYear = x.Show.Year,
                Role = x.Role,
            }).ToList();
            viewModel.CrewPositions = crew.Select(x => new GetViewModel.CrewPosition
            {
                ShowCrewId = x.ShowCrewId,
                ShowId = x.Show.ShowId,
                ShowName = x.Show.Title,
                ShowQuarter = x.Show.Quarter,
                ShowYear = x.Show.Year,
                Name = x.Position,
            }).ToList();
            viewModel.RelatedMediaCount = relatedMedia.Count;
            viewModel.NewRelatedMedia = relatedMedia.OrderByDescending(x => x.InsertedDateTime).Select(x => x.MediaItem.MediaItemId).Where(x => x != person.MediaItem.MediaItemId).Take(4).ToList();

            // for editing -- TODO: only query when the current user can edit this person
            viewModel.AllAwards = DatabaseSession.Query<Award>().Select(x => new GetViewModel.Award
            {
                AwardId = x.AwardId,
                Name = x.Name,
            }).ToList();
            viewModel.AllShows = DatabaseSession.Query<Show>().Select(x => new GetViewModel.Show
            {
                ShowId = x.ShowId,
                ShowTitle = x.Title,
                ShowQuarter = x.Quarter,
                ShowYear = x.Year,
            }).ToList();
            viewModel.UniqueCrewPositions = DatabaseSession.Query<ShowCrew>().Select(x => x.Position).Distinct().ToList();
            viewModel.UniqueClubPositions = DatabaseSession.Query<PersonClubPosition>().Select(x => x.Position).Distinct().ToList();

            return View(viewModel);
        }

        [HttpGet, Route("{personId}/Media")]
        public virtual ActionResult ListPersonMedia(int personId)
        {
            var person = DatabaseSession.Get<Person>(personId);
            var relatedMedia = DatabaseSession.Query<PersonMedia>().Where(x => x.Person == person).Fetch(x => x.MediaItem).ToList();

            var viewModel = new ListPersonMediaViewModel();
            viewModel.PersonId = personId;
            viewModel.PersonFullname = person.Fullname;
            viewModel.RelatedMedia = relatedMedia.OrderBy(x => x.MediaItem.InsertedDateTime).ThenBy(x => x.MediaItem.MediaItemId).Select(x => new ListPersonMediaViewModel.Media
            {
                MediaItemId = x.MediaItem.MediaItemId,
            }).ToList();
            return View(viewModel);
        }

        [HttpGet, Route("{personId}/Media/{mediaItemId}")]
        public virtual ActionResult GetPersonMedia(int personId, int mediaItemId)
        {
            var person = DatabaseSession.Get<Person>(personId);
            var relatedMedia = DatabaseSession
                .Query<PersonMedia>().Where(x => x.Person == person).Fetch(x => x.MediaItem)
                .ToList()
                .OrderBy(x => x.MediaItem.InsertedDateTime).ThenBy(x => x.MediaItem.MediaItemId)
                .ToList();
            var media = relatedMedia.Single(x => x.MediaItem.MediaItemId == mediaItemId);

            var index = relatedMedia.IndexOf(relatedMedia.Single(x => x.PersonMediaId == media.PersonMediaId));
            var previousId = index > 0 ? relatedMedia[index - 1].MediaItem.MediaItemId : (int?)null;
            var nextId = index < relatedMedia.Count - 1 ? relatedMedia[index + 1].MediaItem.MediaItemId : (int?)null;

            var viewModel = new GetPersonMediaViewModel();
            viewModel.PersonId = personId;
            viewModel.PersonFullname = person.Fullname;
            viewModel.PreviousId = previousId;
            viewModel.NextId = nextId;
            viewModel.MediaItemId = media.MediaItem.MediaItemId;
            viewModel.MediaItemViewModel = new MediaItemViewModel();
            viewModel.MediaItemViewModel.PopulateFromDatabase(DatabaseSession, mediaItemId);

            return View(viewModel);
        }

        [HttpGet, Route("New", Order = 0)]
        public virtual ActionResult New()
        {
            var viewModel = EditViewModel.Empty();
            return View("Edit", viewModel);
        }

        [HttpPost, Route("New", Order = 0)]
        public virtual ActionResult SaveNew(SaveEditParameters param)
        {
            if (string.IsNullOrWhiteSpace(param.FirstName) ||
                string.IsNullOrWhiteSpace(param.LastName))
            {
                throw new Exception("Name is required");
            }

            var person = new Person();
            person.FirstName = (param.FirstName ?? string.Empty).Trim();
            person.LastName = (param.LastName ?? string.Empty).Trim();
            person.MiddleName = (param.MiddleName ?? string.Empty).Trim();
            person.Honorific = (param.Honorific ?? string.Empty).Trim();
            person.Suffix = (param.Suffix ?? string.Empty).Trim();
            person.Nickname = (param.Nickname ?? string.Empty).Trim();
            person.Biography = (param.Biography ?? string.Empty).Trim();
            person.MediaItem = DatabaseSession.Load<MediaItem>(MediaItem.NoPic);
            // TODO: build in auditing
            person.InsertedDateTime = DateTime.UtcNow;
            person.LastModifiedDateTime = DateTime.UtcNow;
            DatabaseSession.Save(person);
            DatabaseSession.CommitTransaction();

            return RedirectToAction(Actions.Get(person.PersonId));
        }

        [HttpGet, Route("{personId}/Edit")]
        public virtual ActionResult Edit(int personId)
        {
            var person = DatabaseSession.Get<Person>(personId);
            var relatedMedia = DatabaseSession.Query<PersonMedia>().Where(x => x.Person == person).Fetch(x => x.MediaItem).ToList();

            var viewModel = new EditViewModel();
            viewModel.PersonId = personId;
            viewModel.FirstName = person.FirstName;
            viewModel.LastName = person.LastName;
            viewModel.MiddleName = person.MiddleName;
            viewModel.Honorific = person.Honorific;
            viewModel.Suffix = person.Suffix;
            viewModel.Nickname = person.Nickname;
            viewModel.Biography = person.Biography;
            viewModel.DefaultMediaItemId = person.MediaItem.MediaItemId;
            viewModel.RelatedMedia = relatedMedia.OrderBy(x => x.MediaItem.InsertedDateTime).ThenBy(x => x.MediaItem.MediaItemId).Select(x => new EditViewModel.Media
            {
                MediaItemId = x.MediaItem.MediaItemId,
            }).ToList();
            return View(viewModel);
        }

        [HttpPost, Route("{personId}/Edit")]
        public virtual ActionResult SaveEdit(int personId, SaveEditParameters param)
        {
            if (string.IsNullOrWhiteSpace(param.FirstName) ||
                string.IsNullOrWhiteSpace(param.LastName))
            {
                throw new Exception("Name is required");
            }

            var person = DatabaseSession.Get<Person>(personId);
            person.FirstName = (param.FirstName ?? string.Empty).Trim();
            person.LastName = (param.LastName ?? string.Empty).Trim();
            person.MiddleName = (param.MiddleName ?? string.Empty).Trim();
            person.Honorific = (param.Honorific ?? string.Empty).Trim();
            person.Suffix = (param.Suffix ?? string.Empty).Trim();
            person.Nickname = (param.Nickname ?? string.Empty).Trim();
            person.Biography = (param.Biography ?? string.Empty).Trim();
            if (DatabaseSession.IsDirtyEntity(person))
            {
                // TODO: build in auditing
                person.LastModifiedDateTime = DateTime.UtcNow;
            }
            DatabaseSession.CommitTransaction();

            return RedirectToAction(Actions.Get(personId));
        }

        [HttpPost, Route("{personId}/ChangeDefaultMediaItem")]
        public virtual ActionResult ChangeDefaultMediaItem(int personId, int mediaItemId)
        {
            var person = DatabaseSession.Get<Person>(personId);
            person.MediaItem = DatabaseSession.Load<MediaItem>(mediaItemId);
            if (DatabaseSession.IsDirtyEntity(person))
            {
                // TODO: build in auditing
                person.LastModifiedDateTime = DateTime.UtcNow;
            }
            DatabaseSession.CommitTransaction();

            return RedirectToAction(Actions.Get(personId));
        }

        public class SaveEditParameters
        {
            public string Honorific { get; set; }
            [Required]
            public string FirstName { get; set; }
            public string MiddleName { get; set; }
            [Required]
            public string LastName { get; set; }
            public string Suffix { get; set; }
            public string Nickname { get; set; }
            [AllowHtml()]
            public string Biography { get; set; }
        }

        [HttpPost, Route("{personId}/DeletePersonAward/{personAwardId}")]
        public virtual ActionResult DeletePersonAward(int personId, int personAwardId)
        {
            var award = DatabaseSession.Get<PersonAward>(personAwardId);
            DatabaseSession.Delete(award);
            DatabaseSession.CommitTransaction();

            return RedirectToAction(Actions.Get(personId));
        }

        [HttpPost, Route("{personId}/DeleteShowAward/{showAwardId}")]
        public virtual ActionResult DeleteShowAward(int personId, int showAwardId)
        {
            var award = DatabaseSession.Get<ShowAward>(showAwardId);
            DatabaseSession.Delete(award);
            DatabaseSession.CommitTransaction();

            return RedirectToAction(Actions.Get(personId));
        }

        [HttpPost, Route("{personId}/DeletePersonClubPosition/{personClubPositionId}")]
        public virtual ActionResult DeletePersonClubPosition(int personId, int personClubPositionId)
        {
            var entity = DatabaseSession.Get<PersonClubPosition>(personClubPositionId);
            DatabaseSession.Delete(entity);
            DatabaseSession.CommitTransaction();

            return RedirectToAction(Actions.Get(personId));
        }

        [HttpPost, Route("{personId}/DeleteShowCast/{showCastId}")]
        public virtual ActionResult DeleteShowCast(int personId, int showCastId)
        {
            var entity = DatabaseSession.Get<ShowCast>(showCastId);
            DatabaseSession.Delete(entity);
            DatabaseSession.CommitTransaction();

            return RedirectToAction(Actions.Get(personId));
        }

        [HttpPost, Route("{personId}/DeleteShowCrew/{showCrewId}")]
        public virtual ActionResult DeleteShowCrew(int personId, int showCrewId)
        {
            var entity = DatabaseSession.Get<ShowCrew>(showCrewId);
            DatabaseSession.Delete(entity);
            DatabaseSession.CommitTransaction();

            return RedirectToAction(Actions.Get(personId));
        }
    }
}