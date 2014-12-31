﻿using System;
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
        public virtual ActionResult PersonDetails(int personId)
        {
            var person = DatabaseSession.Get<Person>(personId);
            var clubPositions = DatabaseSession.Query<PersonClubPosition>().Where(x => x.Person == person).ToList();
            var crew = DatabaseSession.Query<ShowCrew>().Where(x => x.Person == person).Fetch(x => x.Show).ToList();
            var cast = DatabaseSession.Query<ShowCast>().Where(x => x.Person == person).Fetch(x => x.Show).ToList();
            var showAwards = DatabaseSession.Query<ShowAward>().Where(x => x.Person == person).Fetch(x => x.Show).Fetch(x => x.Award).ToList();
            var myAwards = DatabaseSession.Query<PersonAward>().Where(x => x.Person == person).Fetch(x => x.Award).ToList();
            var relatedMedia = DatabaseSession.Query<PersonMedia>().Where(x => x.Person == person).Fetch(x => x.MediaItem).ToList();

            var viewModel = new PersonDetailsViewModel();
            viewModel.PersonId = personId;
            viewModel.FullName = person.Fullname;
            viewModel.Biography = person.Biography;
            viewModel.MediaItemId = person.MediaItem.MediaItemId;
            viewModel.ClubPositions = clubPositions.OrderBy(x => x.DisplayOrder).ThenByDescending(x => x.Year).Select(x => new PersonDetailsViewModel.ClubPosition
            {
                Year = x.Year,
                Name = x.Position,
                ClubPositionId = x.PersonClubPositionId,
            }).ToList();
            viewModel.Awards = showAwards.Select(x => new PersonDetailsViewModel.Award
            {
                Year = x.Year,
                AwardId = x.ShowAwardId,
                Name = x.Award.Name,
                ShowId = x.Show.ShowId,
                ShowName = x.Show.Title,
                ShowQuarter = x.Show.Quarter,
                ShowYear = x.Show.Year,
            })
            .Concat(myAwards.Select(x => new PersonDetailsViewModel.Award
            {
                Year = x.Year,
                AwardId = x.PersonAwardId,
                Name = x.Award.Name,
            })).ToList();
            viewModel.CastRoles = cast.Select(x => new PersonDetailsViewModel.CastRole
            {
                ShowCastId = x.ShowCastId,
                ShowId = x.Show.ShowId,
                ShowName = x.Show.Title,
                ShowQuarter = x.Show.Quarter,
                ShowYear = x.Show.Year,
                Role = x.Role,
            }).ToList();
            viewModel.CrewPositions = crew.Select(x => new PersonDetailsViewModel.CrewPosition
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
            var viewModel = EditPersonViewModel.Empty();
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

            return RedirectToAction(Actions.PersonDetails(person.PersonId));
        }

        [HttpGet, Route("{personId}/Edit")]
        public virtual ActionResult EditPerson(int personId)
        {
            if (!Request.IsAjaxRequest())
            {
                return RedirectToAction(Actions.PersonDetails(personId));
            }

            var person = DatabaseSession.Get<Person>(personId);
            var relatedMedia = DatabaseSession.Query<PersonMedia>().Where(x => x.Person == person).Fetch(x => x.MediaItem).ToList();

            var viewModel = new EditPersonViewModel();
            viewModel.PersonId = personId;
            viewModel.FirstName = person.FirstName;
            viewModel.LastName = person.LastName;
            viewModel.MiddleName = person.MiddleName;
            viewModel.Honorific = person.Honorific;
            viewModel.Suffix = person.Suffix;
            viewModel.Nickname = person.Nickname;
            viewModel.Biography = person.Biography;
            viewModel.DefaultMediaItemId = person.MediaItem.MediaItemId;
            viewModel.RelatedMedia = relatedMedia.OrderBy(x => x.MediaItem.InsertedDateTime).ThenBy(x => x.MediaItem.MediaItemId).Select(x => new EditPersonViewModel.Media
            {
                MediaItemId = x.MediaItem.MediaItemId,
            }).ToList();

            return PartialView(viewModel);
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

            return RedirectToAction(Actions.PersonDetails(personId));
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

            return RedirectToAction(Actions.PersonDetails(personId));
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

        [HttpGet, Route("{personId}/AddAward")]
        public virtual ActionResult AddAward(int personId)
        {
            if (!Request.IsAjaxRequest())
            {
                return RedirectToAction(Actions.PersonDetails(personId));
            }

            var person = DatabaseSession.Get<Person>(personId);
            var awards = DatabaseSession.Query<Award>()
                .ToList()
                .OrderBy(x => x.Name)
                .Select(x => new
                {
                    Id = x.AwardId,
                    Name = x.Name,
                }.ToExpando())
                .ToList();
            var shows = DatabaseSession.Query<Show>()
                .ToList()
                .OrderBy(x => x.Title)
                .Select(x => new
                {
                    ShowId = x.ShowId,
                    Quarter = x.Quarter,
                    Title = x.Title,
                    Year = x.Year,
                }.ToExpando())
                .ToList();
            var viewModel = new
            {
                Shows = shows,
                Awards = awards,
            }.ToExpando();
            return PartialView(viewModel);
        }

        [HttpPost, Route("{personId}/AddAward")]
        public virtual ActionResult AddAward(int personId, int awardId, short year, int? showId)
        {
            if (showId == null)
            {
                var award = new PersonAward(DatabaseSession.Load<Person>(personId), DatabaseSession.Load<Award>(awardId), year);
                DatabaseSession.Save(award);
                DatabaseSession.CommitTransaction();
            }
            else
            {
                var award = new ShowAward(DatabaseSession.Load<Show>(showId.Value), DatabaseSession.Load<Person>(personId), DatabaseSession.Load<Award>(awardId), year);
                DatabaseSession.Save(award);
                DatabaseSession.CommitTransaction();
            }
            return RedirectToAction(Actions.PersonDetails(personId));
        }

        [HttpPost, Route("{personId}/DeleteAward")]
        public virtual ActionResult DeleteAward(int personId, int? personAwardId, int? showAwardId)
        {
            if (personAwardId.HasValue)
            {
                var award = DatabaseSession.Get<PersonAward>(personAwardId.Value);
                DatabaseSession.Delete(award);
                DatabaseSession.CommitTransaction();
            }
            if (showAwardId.HasValue)
            {
                var award = DatabaseSession.Get<ShowAward>(showAwardId.Value);
                DatabaseSession.Delete(award);
                DatabaseSession.CommitTransaction();
            }
            return RedirectToAction(Actions.PersonDetails(personId));
        }

        [HttpGet, Route("{personId}/AddClubPosition")]
        public virtual ActionResult AddClubPosition(int personId)
        {
            if (!Request.IsAjaxRequest())
            {
                return RedirectToAction(Actions.PersonDetails(personId));
            }
            var person = DatabaseSession.Get<Person>(personId);
            var positions = DatabaseSession.Query<PersonClubPosition>().Select(x => x.Position).Distinct().ToList();
            var viewModel = new
            {
                Positions = string.Join(", ", positions.OrderBy(x => x).Select(x => "\"" + x.Replace("\"", "&quot;") + "\"")),
            }.ToExpando();
            return PartialView(viewModel);
        }

        [HttpPost, Route("{personId}/AddClubPosition")]
        public virtual ActionResult AddClubPosition(int personId, string position, short year)
        {
            var entity = new PersonClubPosition(DatabaseSession.Load<Person>(personId), position, year);
            DatabaseSession.Save(entity);
            DatabaseSession.CommitTransaction();
            return RedirectToAction(Actions.PersonDetails(personId));
        }

        [HttpPost, Route("{personId}/DeleteClubPosition")]
        public virtual ActionResult DeleteClubPosition(int personId, int personClubPositionId)
        {
            var entity = DatabaseSession.Get<PersonClubPosition>(personClubPositionId);
            DatabaseSession.Delete(entity);
            DatabaseSession.CommitTransaction();
            return RedirectToAction(Actions.PersonDetails(personId));
        }

        [HttpGet, Route("{personId}/AddCast")]
        public virtual ActionResult AddCast(int personId)
        {
            if (!Request.IsAjaxRequest())
            {
                return RedirectToAction(Actions.PersonDetails(personId));
            }

            var person = DatabaseSession.Get<Person>(personId);
            var shows = DatabaseSession.Query<Show>()
                .ToList()
                .OrderBy(x => x.Title)
                .Select(x => new
                {
                    ShowId = x.ShowId,
                    Quarter = x.Quarter,
                    Title = x.Title,
                    Year = x.Year,
                }.ToExpando())
                .ToList();
            var viewModel = new
            {
                Shows = shows,
            }.ToExpando();
            return PartialView(viewModel);
        }

        [HttpPost, Route("{personId}/AddCast")]
        public virtual ActionResult AddCast(int personId, int showId, string role)
        {
            var entity = new ShowCast(DatabaseSession.Load<Person>(personId), DatabaseSession.Load<Show>(showId), role);
            DatabaseSession.Save(entity);
            DatabaseSession.CommitTransaction();
            return RedirectToAction(Actions.PersonDetails(personId));
        }

        [HttpPost, Route("{personId}/DeleteCast")]
        public virtual ActionResult DeleteCast(int personId, int showCastId)
        {
            var entity = DatabaseSession.Get<ShowCast>(showCastId);
            DatabaseSession.Delete(entity);
            DatabaseSession.CommitTransaction();
            return RedirectToAction(Actions.PersonDetails(personId));
        }

        [HttpGet, Route("{personId}/AddCrew")]
        public virtual ActionResult AddCrew(int personId)
        {
            if (!Request.IsAjaxRequest())
            {
                return RedirectToAction(Actions.PersonDetails(personId));
            }

            var person = DatabaseSession.Get<Person>(personId);
            var shows = DatabaseSession.Query<Show>()
                .ToList()
                .OrderBy(x => x.Title)
                .Select(x => new
                {
                    ShowId = x.ShowId,
                    Quarter = x.Quarter,
                    Title = x.Title,
                    Year = x.Year,
                }.ToExpando())
                .ToList();
            var positions = DatabaseSession.Query<ShowCrew>().Select(x => x.Position).Distinct().ToList();
            var viewModel = new
            {
                Shows = shows,
                Positions = string.Join(", ", positions.OrderBy(x => x).Select(x => "\"" + x.Replace("\\", "\\\\").Replace("\"", "&quot;") + "\"")),
            }.ToExpando();
            return PartialView(viewModel);
        }

        [HttpPost, Route("{personId}/AddCrew")]
        public virtual ActionResult AddCrew(int personId, int showId, string position)
        {
            var entity = new ShowCrew(DatabaseSession.Load<Person>(personId), DatabaseSession.Load<Show>(showId), position);
            DatabaseSession.Save(entity);
            DatabaseSession.CommitTransaction();
            return RedirectToAction(Actions.PersonDetails(personId));
        }

        [HttpPost, Route("{personId}/DeleteCrew")]
        public virtual ActionResult DeleteCrew(int personId, int showCrewId)
        {
            var entity = DatabaseSession.Get<ShowCrew>(showCrewId);
            DatabaseSession.Delete(entity);
            DatabaseSession.CommitTransaction();
            return RedirectToAction(Actions.PersonDetails(personId));
        }
    }
}