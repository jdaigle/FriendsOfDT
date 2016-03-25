using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using FODT.Database;
using FODT.Models.IMDT;
using FODT.Views.Person;
using FODT.Views.Photos;
using FODT.Security;
using NHibernate.Linq;
using FODT.Views.Awards;
using FODT.Views.Show;
using Microsoft.Web.Mvc;
using System.Net;

namespace FODT.Controllers
{
    [RoutePrefix("archive/Person")]
    public class PersonController : BaseController
    {
        [HttpGet, Route("{personId}")]
        public ActionResult PersonDetails(int personId)
        {
            var person = DatabaseSession.Get<Person>(personId);
            var clubPositions = DatabaseSession.Query<PersonClubPosition>().Where(x => x.Person == person).ToList();
            var crew = DatabaseSession.Query<ShowCrew>().Where(x => x.Person == person).Fetch(x => x.Show).ToList();
            var cast = DatabaseSession.Query<ShowCast>().Where(x => x.Person == person).Fetch(x => x.Show).ToList();
            var awards = DatabaseSession.Query<Award>().Where(x => x.Person == person).Fetch(x => x.Show).Fetch(x => x.AwardType).ToList();
            var photos = DatabaseSession.Query<PersonPhoto>().Where(x => x.Person == person).Fetch(x => x.Photo).ToList();

            var viewModel = new PersonDetailsViewModel();

            viewModel.EditLinkURL = this.GetURL(c => c.EditPerson(personId));
            viewModel.ShowPhotoUploadControl = User.IsInRole(RoleNames.Contributor) || User.IsInRole(RoleNames.Archivist);
            viewModel.PhotoUploadLinkURL = this.GetURL<PersonPhotosController>(c => c.Upload(personId));
            viewModel.PhotoLinkURL = this.GetURL<PersonPhotosController>(c => c.ListPersonPhotos(personId, person.Photo.PhotoId));
            viewModel.PhotoThumbnailURL = person.Photo.GetThumbnailFileURL();
            viewModel.PhotoListLinkURL = this.GetURL<PersonPhotosController>(c => c.ListPersonPhotos(personId, null));

            viewModel.FullName = person.Fullname;
            viewModel.Biography = person.Biography;

            viewModel.AwardsTable = new AwardsTableViewModel(
                this.Url
                , id => this.GetURL(c => c.DeleteAward(personId, id))
                , awards)
            {
                CanEdit = this.ControllerContext.CanEditPerson(person),
                AddItemURL = this.GetURL(c => c.AddAward(personId)),
            };

            viewModel.ClubPositionsTable = new ClubPositionsTableViewModel(
                this.Url
                , id => this.GetURL(c => c.DeleteClubPosition(personId, id))
                , clubPositions)
            {
                CanEdit = this.ControllerContext.CanEditPerson(person),
                AddItemURL = this.GetURL(c => c.AddClubPosition(personId)),
            };

            viewModel.CastRolesTable = new CastRolesTableViewModel(
                this.Url
                , id => this.GetURL(c => c.DeleteCast(personId, id))
                , cast)
            {
                CanEdit = this.ControllerContext.CanEditPerson(person),
                AddItemURL = this.GetURL(c => c.AddCast(personId)),
            };

            viewModel.CrewPositionsTable = new CrewPositionsTableViewModel(
                this.Url
                , id => this.GetURL(c => c.DeleteCrew(personId, id))
                , crew)
            {
                CanEdit = this.ControllerContext.CanEditPerson(person),
                AddItemURL = this.GetURL(c => c.AddCrew(personId)),
            };

            viewModel.PhotoCount = photos.Count;
            viewModel.NewPhotos = photos
                .OrderByDescending(x => x.InsertedDateTime)
                .Where(x => x.Photo.PhotoId != person.Photo.PhotoId)
                .Select(x => new PersonDetailsViewModel.NewPhotosViewModel
                {
                    PhotoLinkURL = this.GetURL<PersonPhotosController>(c => c.ListPersonPhotos(personId, x.Photo.PhotoId)),
                    PhotoTinyURL = x.Photo.GetTinyFileURL(),
                })
                .Take(4)
                .ToList();

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
            return PartialView(viewModel);
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
            person.Photo = DatabaseSession.Load<Photo>(Photo.NoPic);
            // TODO: build in auditing
            person.InsertedDateTime = DateTime.UtcNow;
            person.LastModifiedDateTime = DateTime.UtcNow;
            DatabaseSession.Save(person);
            DatabaseSession.Flush();

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
            var photos = DatabaseSession.Query<PersonPhoto>().Where(x => x.Person == person).Fetch(x => x.Photo).ToList();

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
            viewModel.DefaultPhotoId = person.Photo.PhotoId;
            viewModel.Photos = photos.OrderBy(x => x.Photo.InsertedDateTime).ThenBy(x => x.Photo.PhotoId).Select(x => new EditPersonViewModel.Photo
            {
                PhotoItemId = x.Photo.PhotoId,
                PhotoTinyURL = x.Photo.GetTinyFileURL(),
                PhotoThumbnailURL = x.Photo.GetThumbnailFileURL(),
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

            return this.RedirectToAction(x => x.PersonDetails(personId));
        }

        [HttpPost, Route("{personId}/ChangeDefaultPhoto")]
        public ActionResult ChangeDefaultPhoto(int personId, int photoId)
        {
            var person = DatabaseSession.Get<Person>(personId);
            person.Photo = DatabaseSession.Load<Photo>(photoId);
            if (DatabaseSession.IsDirtyEntity(person))
            {
                // TODO: build in auditing
                person.LastModifiedDateTime = DateTime.UtcNow;
            }

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

        [HttpGet, AjaxOnly, Route("{personId}/Awards/Add")]
        [Authorize(Roles = RoleNames.Archivist)]
        public ActionResult AddAward(int personId)
        {
            var person = DatabaseSession.Get<Person>(personId);
            var awardTypes = DatabaseSession.Query<AwardType>().ToList();
            var shows = DatabaseSession.Query<Show>().ToList();
            var viewModel = new AddAwardViewModel(awardTypes, shows)
            {
                POSTUrl = this.GetURL(x => x.AddAward(personId)),
            };
            return new ViewModelResult(viewModel);
        }

        [HttpPost, Route("{personId}/Awards/Add")]
        [Authorize(Roles = RoleNames.Archivist)]
        public ActionResult AddAward(int personId, int awardTypeId, short year, int? showId)
        {
            Show show = showId.HasValue ? DatabaseSession.Load<Show>(showId.Value) : null;
            var award = new Award(show, DatabaseSession.Load<Person>(personId), DatabaseSession.Load<AwardType>(awardTypeId), year);
            DatabaseSession.Save(award);
            return new ViewModelResult(new HttpApiResult
            {
                Message = "Award Added",
                RedirectToURL = this.GetURL(c => c.PersonDetails(personId)),
            });
        }

        [HttpPost, Route("{personId}/Awards/{awardId}/Delete")]
        [Authorize(Roles = RoleNames.Archivist)]
        public ActionResult DeleteAward(int personId, int awardId)
        {
            var award = DatabaseSession.Get<Award>(awardId);
            DatabaseSession.Delete(award);
            return new ViewModelResult(new HttpApiResult
            {
                Message = "Award Deleted",
                RedirectToURL = this.GetURL(c => c.PersonDetails(personId)),
            });
        }

        [HttpGet, AjaxOnly, Route("{personId}/ClubPositions/Add")]
        [Authorize(Roles = RoleNames.Archivist)]
        public ActionResult AddClubPosition(int personId)
        {
            if (!Request.IsAjaxRequest())
            {
                return this.RedirectToAction(x => x.PersonDetails(personId));
            }
            var person = DatabaseSession.Get<Person>(personId);
            var positions = DatabaseSession.Query<PersonClubPosition>().Select(x => x.Position).Distinct().ToList();
            var viewModel = new AddClubPositionViewModel(positions)
            {
                POSTUrl = this.GetURL(x => x.AddClubPosition(personId)),
            };
            return new ViewModelResult(viewModel);
        }

        [HttpPost, Route("{personId}/ClubPositions/Add")]
        [Authorize(Roles = RoleNames.Archivist)]
        public ActionResult AddClubPosition(int personId, string position, short year)
        {
            var entity = new PersonClubPosition(DatabaseSession.Load<Person>(personId), position, year);
            DatabaseSession.Save(entity);
            return new ViewModelResult(new HttpApiResult
            {
                RedirectToURL = this.GetURL(c => c.PersonDetails(personId)),
            });
        }

        [HttpPost, Route("{personId}/ClubPositions/{personClubPositionId}/Delete")]
        [Authorize(Roles = RoleNames.Archivist)]
        public ActionResult DeleteClubPosition(int personId, int personClubPositionId)
        {
            var entity = DatabaseSession.Get<PersonClubPosition>(personClubPositionId);
            DatabaseSession.Delete(entity);
            return new ViewModelResult(new HttpApiResult
            {
                RedirectToURL = this.GetURL(c => c.PersonDetails(personId)),
            });
        }

        [HttpGet, Route("{personId}/AddCast")]
        [Authorize(Roles = RoleNames.Archivist)]
        public ActionResult AddCast(int personId)
        {
            if (!Request.IsAjaxRequest())
            {
                return this.RedirectToAction(x => x.PersonDetails(personId));
            }

            var person = DatabaseSession.Get<Person>(personId);
            var shows = DatabaseSession.Query<Show>().ToList();
            var viewModel = new AddCastViewModel(shows)
            {
                POSTUrl = this.GetURL(x => x.AddCast(personId)),
            };
            return new ViewModelResult(viewModel);
        }

        [HttpPost, Route("{personId}/AddCast")]
        [Authorize(Roles = RoleNames.Archivist)]
        public ActionResult AddCast(int personId, int showId, string role)
        {
            var entity = new ShowCast(DatabaseSession.Load<Person>(personId), DatabaseSession.Load<Show>(showId), role);
            DatabaseSession.Save(entity);
            return new ViewModelResult(new HttpApiResult
            {
                RedirectToURL = this.GetURL(c => c.PersonDetails(personId)),
            });
        }

        [HttpPost, Route("{personId}/DeleteCast")]
        [Authorize(Roles = RoleNames.Archivist)]
        public ActionResult DeleteCast(int personId, int showCastId)
        {
            var entity = DatabaseSession.Get<ShowCast>(showCastId);
            DatabaseSession.Delete(entity);
            return new ViewModelResult(new HttpApiResult
            {
                RedirectToURL = this.GetURL(c => c.PersonDetails(personId)),
            });
        }

        [HttpGet, Route("{personId}/AddCrew")]
        [Authorize(Roles = RoleNames.Archivist)]
        public ActionResult AddCrew(int personId)
        {
            if (!Request.IsAjaxRequest())
            {
                return this.RedirectToAction(x => x.PersonDetails(personId));
            }

            var person = DatabaseSession.Get<Person>(personId);
            var shows = DatabaseSession.Query<Show>().ToList();
            var crewPositions = DatabaseSession.Query<ShowCrew>().Select(x => x.Position).Distinct().ToList();
            var viewModel = new AddCrewViewModel(shows, crewPositions)
            {
                POSTUrl = this.GetURL(x => x.AddCrew(personId)),
            };
            return new ViewModelResult(viewModel);
        }

        [HttpPost, Route("{personId}/AddCrew")]
        [Authorize(Roles = RoleNames.Archivist)]
        public ActionResult AddCrew(int personId, int showId, string position)
        {
            var entity = new ShowCrew(DatabaseSession.Load<Person>(personId), DatabaseSession.Load<Show>(showId), position);
            DatabaseSession.Save(entity);
            return new ViewModelResult(new HttpApiResult
            {
                RedirectToURL = this.GetURL(c => c.PersonDetails(personId)),
            });
        }

        [HttpPost, Route("{personId}/DeleteCrew")]
        [Authorize(Roles = RoleNames.Archivist)]
        public ActionResult DeleteCrew(int personId, int showCrewId)
        {
            var entity = DatabaseSession.Get<ShowCrew>(showCrewId);
            DatabaseSession.Delete(entity);
            return new ViewModelResult(new HttpApiResult
            {
                RedirectToURL = this.GetURL(c => c.PersonDetails(personId)),
            });
        }
    }
}