using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using FODT.Database;
using FODT.Models.IMDT;
using FODT.Views.Person;
using FODT.Views.Shared;
using FODT.Security;
using NHibernate.Linq;
using FODT.Views.Awards;
using FODT.Views.Show;

namespace FODT.Controllers
{
    [RoutePrefix("Person")]
    public class PersonController : BaseController
    {
        [HttpGet, Route("{personId}")]
        public ActionResult PersonDetails(int personId)
        {
            var person = DatabaseSession.Get<Person>(personId);
            var clubPositions = DatabaseSession.Query<PersonClubPosition>().Where(x => x.Person == person).ToList();
            var crew = DatabaseSession.Query<ShowCrew>().Where(x => x.Person == person).Fetch(x => x.Show).ToList();
            var cast = DatabaseSession.Query<ShowCast>().Where(x => x.Person == person).Fetch(x => x.Show).ToList();
            var showAwards = DatabaseSession.Query<ShowAward>().Where(x => x.Person == person).Fetch(x => x.Show).Fetch(x => x.Award).ToList();
            var myAwards = DatabaseSession.Query<PersonAward>().Where(x => x.Person == person).Fetch(x => x.Award).ToList();
            var relatedMedia = DatabaseSession.Query<PersonMedia>().Where(x => x.Person == person).Fetch(x => x.MediaItem).ToList();

            var viewModel = new PersonDetailsViewModel();

            viewModel.CanEdit = this.ControllerContext.CanEditPerson(person);

            viewModel.EditLinkURL = this.GetURL(c => c.EditPerson(personId));
            viewModel.MediaUploadLinkURL = this.GetURL<MediaController>(c => c.Upload());
            viewModel.MediaLinkURL = this.GetURL(c => c.GetPersonMedia(personId, person.MediaItem.MediaItemId));
            viewModel.MediaThumbnailURL = this.GetURL<MediaController>(c => c.GetItemThumbnail(person.MediaItem.MediaItemId));
            viewModel.MediaListLinkURL = this.GetURL(c => c.ListPersonMedia(personId));

            viewModel.PersonId = personId;
            viewModel.FullName = person.Fullname;
            viewModel.Biography = person.Biography;
            viewModel.MediaItemId = person.MediaItem.MediaItemId;

            viewModel.AwardsTable = new AwardsTableViewModel(
                this.Url
                , (x, y) => this.GetURL(c => c.DeleteAward(personId, x, y))
                , showAwards: showAwards
                , personAwards: myAwards)
            {
                CanEdit = this.ControllerContext.CanEditPerson(person),
                AddItemURL = this.GetURL(c => c.AddAward(personId)),
            };

            viewModel.ClubPositionsTable = new ClubPositionsTableViewModel(
                this.Url
                , x => this.GetURL(c => c.DeleteClubPosition(personId, x))
                , clubPositions)
            {
                CanEdit = this.ControllerContext.CanEditPerson(person),
                AddItemURL = this.GetURL(c => c.AddClubPosition(personId)),
            };

            viewModel.CastRolesTable = new CastRolesTableViewModel(
                this.Url
                , x => this.GetURL(c => c.DeleteCast(personId, x))
                , cast)
            {
                CanEdit = this.ControllerContext.CanEditPerson(person),
                AddItemURL = this.GetURL(c => c.AddCast(personId)),
            };

            viewModel.CrewPositionsTable = new CrewPositionsTableViewModel(
                this.Url
                , x => this.GetURL(c => c.DeleteCrew(personId, x))
                , crew)
            {
                CanEdit = this.ControllerContext.CanEditPerson(person),
                AddItemURL = this.GetURL(c => c.AddCrew(personId)),
            };

            viewModel.RelatedMediaCount = relatedMedia.Count;
            viewModel.NewRelatedMedia = relatedMedia
                .OrderByDescending(x => x.InsertedDateTime)
                .Where(x => x.MediaItem.MediaItemId != person.MediaItem.MediaItemId)
                .Select(x => new PersonDetailsViewModel.RelatedMediaViewModel
                {
                    ID = x.MediaItem.MediaItemId,
                    MediaLinkURL = this.GetURL(c => c.GetPersonMedia(personId, x.MediaItem.MediaItemId)),
                    MediaThumbnailURL = this.GetURL<MediaController>(c => c.GetItemTiny(x.MediaItem.MediaItemId)),
                })
                .Take(4)
                .ToList();

            return View(viewModel);
        }

        [HttpGet, Route("{personId}/Media")]
        public ActionResult ListPersonMedia(int personId)
        {
            var person = DatabaseSession.Get<Person>(personId);
            var relatedMedia = DatabaseSession.Query<PersonMedia>().Where(x => x.Person == person).Fetch(x => x.MediaItem).ToList();

            var viewModel = new ListPersonMediaViewModel();
            viewModel.PersonFullname = person.Fullname;
            viewModel.MediaUploadLinkURL = this.GetURL<MediaController>(c => c.Upload());
            viewModel.PersonLinkURL = this.GetURL(c => c.PersonDetails(personId));
            viewModel.RelatedMedia = relatedMedia.OrderBy(x => x.MediaItem.InsertedDateTime).ThenBy(x => x.MediaItem.MediaItemId).Select(x => new ListPersonMediaViewModel.Media
            {
                MediaLinkURL = this.GetURL(c => c.GetPersonMedia(personId, x.MediaItem.MediaItemId)),
                MediaThumbnailURL = this.GetURL<MediaController>(c => c.GetItemThumbnail(x.MediaItem.MediaItemId)),
            }).ToList();
            return View(viewModel);
        }

        [HttpGet, Route("{personId}/Media/{mediaItemId}")]
        public ActionResult GetPersonMedia(int personId, int mediaItemId)
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
            viewModel.PersonFullname = person.Fullname;
            viewModel.MediaItemViewModel = new MediaItemViewModel();
            viewModel.MediaItemViewModel.PopulateFromDatabase(DatabaseSession, Url, mediaItemId);

            viewModel.MediaUploadLinkURL = this.GetURL<MediaController>(c => c.Upload());
            viewModel.PersonLinkURL = this.GetURL(c => c.PersonDetails(personId));
            viewModel.PreviousMediaLinkURL = this.GetURL(c => c.GetPersonMedia(personId, mediaItemId));
            viewModel.NextMediaLinkURL = this.GetURL(c => c.GetPersonMedia(personId, mediaItemId));
            if (previousId.HasValue)
            {
                viewModel.HasPreviousMediaLinkURL = true;
                viewModel.PreviousMediaLinkURL = this.GetURL(c => c.GetPersonMedia(personId, previousId.Value));
            }
            if (nextId.HasValue)
            {
                viewModel.HasNextMediaLinkURL = true;
                viewModel.NextMediaLinkURL = this.GetURL(c => c.GetPersonMedia(personId, nextId.Value));
            }

            return View(viewModel);
        }

        [HttpGet, Route("Add")]
        public ActionResult AddPerson()
        {
            if (!Request.IsAjaxRequest())
            {
                return Redirect("~");
            }
            var viewModel = new EditPersonViewModel();
            viewModel.POSTUrl = Url.Action("SaveAddPerson");
            return PartialView("EditPerson", viewModel);
        }

        [HttpPost, Route("Add")]
        public ActionResult SaveAddPerson(SaveEditParameters param)
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

            return this.RedirectToAction(x => x.PersonDetails(person.PersonId));
        }

        [HttpGet, Route("{personId}/Edit")]
        public ActionResult EditPerson(int personId)
        {
            if (!Request.IsAjaxRequest())
            {
                return this.RedirectToAction(x => x.PersonDetails(personId));
            }

            var person = DatabaseSession.Get<Person>(personId);
            var relatedMedia = DatabaseSession.Query<PersonMedia>().Where(x => x.Person == person).Fetch(x => x.MediaItem).ToList();

            var viewModel = new EditPersonViewModel();
            viewModel.POSTUrl = Url.Action("SaveEdit");
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
                MediaTinyURL = this.GetURL<MediaController>(c => c.GetItemTiny(x.MediaItem.MediaItemId)),
                MediaThumbnailURL = this.GetURL<MediaController>(c => c.GetItemThumbnail(x.MediaItem.MediaItemId)),
            }).ToList();

            return PartialView(viewModel);
        }

        [HttpPost, Route("{personId}/Edit")]
        public ActionResult SaveEdit(int personId, SaveEditParameters param)
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

            return this.RedirectToAction(x => x.PersonDetails(personId));
        }

        [HttpPost, Route("{personId}/ChangeDefaultMediaItem")]
        public ActionResult ChangeDefaultMediaItem(int personId, int mediaItemId)
        {
            var person = DatabaseSession.Get<Person>(personId);
            person.MediaItem = DatabaseSession.Load<MediaItem>(mediaItemId);
            if (DatabaseSession.IsDirtyEntity(person))
            {
                // TODO: build in auditing
                person.LastModifiedDateTime = DateTime.UtcNow;
            }
            DatabaseSession.CommitTransaction();

            return this.RedirectToAction(x => x.PersonDetails(personId));
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
        public ActionResult AddAward(int personId)
        {
            if (!Request.IsAjaxRequest())
            {
                return this.RedirectToAction(x => x.PersonDetails(personId));
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
                POSTUrl = this.GetURL(x => x.AddAward(personId)),
                Shows = shows,
                Awards = awards,
            }.ToExpando();
            return PartialView(viewModel);
        }

        [HttpPost, Route("{personId}/AddAward")]
        public ActionResult AddAward(int personId, int awardId, short year, int? showId)
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
            return this.RedirectToAction(x => x.PersonDetails(personId));
        }

        [HttpPost, Route("{personId}/DeleteAward")]
        public ActionResult DeleteAward(int personId, int awardId, int? showId)
        {
            if (showId.HasValue)
            {
                var award = DatabaseSession.Get<ShowAward>(awardId);
                DatabaseSession.Delete(award);
                DatabaseSession.CommitTransaction();
            }
            else
            {
                var award = DatabaseSession.Get<PersonAward>(awardId);
                DatabaseSession.Delete(award);
                DatabaseSession.CommitTransaction();
            }
            return this.RedirectToAction(x => x.PersonDetails(personId));
        }

        [HttpGet, Route("{personId}/AddClubPosition")]
        public ActionResult AddClubPosition(int personId)
        {
            if (!Request.IsAjaxRequest())
            {
                return this.RedirectToAction(x => x.PersonDetails(personId));
            }
            var person = DatabaseSession.Get<Person>(personId);
            var positions = DatabaseSession.Query<PersonClubPosition>().Select(x => x.Position).Distinct().ToList();
            var viewModel = new
            {
                POSTUrl = this.GetURL(x => x.AddClubPosition(personId)),
                Positions = string.Join(", ", positions.OrderBy(x => x).Select(x => "\"" + x.Replace("\"", "&quot;") + "\"")),
            }.ToExpando();
            return PartialView(viewModel);
        }

        [HttpPost, Route("{personId}/AddClubPosition")]
        public ActionResult AddClubPosition(int personId, string position, short year)
        {
            var entity = new PersonClubPosition(DatabaseSession.Load<Person>(personId), position, year);
            DatabaseSession.Save(entity);
            DatabaseSession.CommitTransaction();
            return this.RedirectToAction(x => x.PersonDetails(personId));
        }

        [HttpPost, Route("{personId}/DeleteClubPosition")]
        public ActionResult DeleteClubPosition(int personId, int personClubPositionId)
        {
            var entity = DatabaseSession.Get<PersonClubPosition>(personClubPositionId);
            DatabaseSession.Delete(entity);
            DatabaseSession.CommitTransaction();
            return this.RedirectToAction(x => x.PersonDetails(personId));
        }

        [HttpGet, Route("{personId}/AddCast")]
        public ActionResult AddCast(int personId)
        {
            if (!Request.IsAjaxRequest())
            {
                return this.RedirectToAction(x => x.PersonDetails(personId));
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
                POSTUrl = this.GetURL(x => x.AddCast(personId)),
                Shows = shows,
            }.ToExpando();
            return PartialView(viewModel);
        }

        [HttpPost, Route("{personId}/AddCast")]
        public ActionResult AddCast(int personId, int showId, string role)
        {
            var entity = new ShowCast(DatabaseSession.Load<Person>(personId), DatabaseSession.Load<Show>(showId), role);
            DatabaseSession.Save(entity);
            DatabaseSession.CommitTransaction();
            return this.RedirectToAction(x => x.PersonDetails(personId));
        }

        [HttpPost, Route("{personId}/DeleteCast")]
        public ActionResult DeleteCast(int personId, int showCastId)
        {
            var entity = DatabaseSession.Get<ShowCast>(showCastId);
            DatabaseSession.Delete(entity);
            DatabaseSession.CommitTransaction();
            return this.RedirectToAction(x => x.PersonDetails(personId));
        }

        [HttpGet, Route("{personId}/AddCrew")]
        public ActionResult AddCrew(int personId)
        {
            if (!Request.IsAjaxRequest())
            {
                return this.RedirectToAction(x => x.PersonDetails(personId));
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
                POSTUrl = this.GetURL(x => x.AddCrew(personId)),
                Shows = shows,
                Positions = string.Join(", ", positions.OrderBy(x => x).Select(x => "\"" + x.Replace("\\", "\\\\").Replace("\"", "&quot;") + "\"")),
            }.ToExpando();
            return PartialView(viewModel);
        }

        [HttpPost, Route("{personId}/AddCrew")]
        public ActionResult AddCrew(int personId, int showId, string position)
        {
            var entity = new ShowCrew(DatabaseSession.Load<Person>(personId), DatabaseSession.Load<Show>(showId), position);
            DatabaseSession.Save(entity);
            DatabaseSession.CommitTransaction();
            return this.RedirectToAction(x => x.PersonDetails(personId));
        }

        [HttpPost, Route("{personId}/DeleteCrew")]
        public ActionResult DeleteCrew(int personId, int showCrewId)
        {
            var entity = DatabaseSession.Get<ShowCrew>(showCrewId);
            DatabaseSession.Delete(entity);
            DatabaseSession.CommitTransaction();
            return this.RedirectToAction(x => x.PersonDetails(personId));
        }
    }
}