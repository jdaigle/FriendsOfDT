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
            var awards = DatabaseSession.Query<Award>().Where(x => x.Person == person).Fetch(x => x.Show).Fetch(x => x.AwardType).ToList();
            var photos = DatabaseSession.Query<PersonPhoto>().Where(x => x.Person == person).Fetch(x => x.Photo).ToList();

            var viewModel = new PersonDetailsViewModel();

            viewModel.EditLinkURL = this.GetURL(c => c.EditPerson(personId));
            viewModel.PhotoUploadLinkURL = this.GetURL<PhotosController>(c => c.Upload());
            viewModel.PhotoLinkURL = this.GetURL(c => c.GetPersonPhoto(personId, person.Photo.PhotoId));
            viewModel.PhotoThumbnailURL = this.GetURL<PhotosController>(c => c.GetPhotoThumbnail(person.Photo.PhotoId));
            viewModel.PhotoListLinkURL = this.GetURL(c => c.ListPersonPhotos(personId));

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
                    PhotoLinkURL = this.GetURL(c => c.GetPersonPhoto(personId, x.Photo.PhotoId)),
                    PhotoTinyURL = this.GetURL<PhotosController>(c => c.GetPhotoTiny(x.Photo.PhotoId)),
                })
                .Take(4)
                .ToList();

            return View(viewModel);
        }

        [HttpGet, Route("{personId}/Photos")]
        public ActionResult ListPersonPhotos(int personId)
        {
            var person = DatabaseSession.Get<Person>(personId);
            var photos = DatabaseSession.Query<PersonPhoto>().Where(x => x.Person == person).Fetch(x => x.Photo).ToList();

            var viewModel = new ListPersonPhotosViewModel();
            viewModel.PersonFullname = person.Fullname;
            viewModel.PhotoUploadLinkURL = this.GetURL<PhotosController>(c => c.Upload());
            viewModel.PersonLinkURL = this.GetURL(c => c.PersonDetails(personId));
            viewModel.Photos = photos.OrderBy(x => x.Photo.InsertedDateTime).ThenBy(x => x.Photo.PhotoId).Select(x => new ListPersonPhotosViewModel.Photo
            {
                PhotoLinkURL = this.GetURL(c => c.GetPersonPhoto(personId, x.Photo.PhotoId)),
                PhotoThumbnailURL = this.GetURL<PhotosController>(c => c.GetPhotoThumbnail(x.Photo.PhotoId)),
            }).ToList();
            return View(viewModel);
        }

        [HttpGet, Route("{personId}/Photo/{photoId}")]
        public ActionResult GetPersonPhoto(int personId, int photoId)
        {
            var person = DatabaseSession.Get<Person>(personId);
            var photos = DatabaseSession
                .Query<PersonPhoto>().Where(x => x.Person == person).Fetch(x => x.Photo)
                .ToList()
                .OrderBy(x => x.Photo.InsertedDateTime).ThenBy(x => x.Photo.PhotoId)
                .ToList();
            var photo = photos.Single(x => x.Photo.PhotoId == photoId);

            var index = photos.IndexOf(photos.Single(x => x.PersonPhotoId == photo.PersonPhotoId));
            var previousId = index > 0 ? photos[index - 1].Photo.PhotoId : (int?)null;
            var nextId = index < photos.Count - 1 ? photos[index + 1].Photo.PhotoId : (int?)null;

            var viewModel = new GetPersonPhotoViewModel();
            viewModel.PersonFullname = person.Fullname;
            viewModel.PhotoViewModel = new PhotoViewModel();
            viewModel.PhotoViewModel.PopulateFromDatabase(DatabaseSession, Url, photoId);

            viewModel.PhotoUploadLinkURL = this.GetURL<PhotosController>(c => c.Upload());
            viewModel.PersonLinkURL = this.GetURL(c => c.PersonDetails(personId));
            viewModel.PreviousPhotoLinkURL = this.GetURL(c => c.GetPersonPhoto(personId, photoId));
            viewModel.NextPhotoLinkURL = this.GetURL(c => c.GetPersonPhoto(personId, photoId));
            if (previousId.HasValue)
            {
                viewModel.HasPreviousPhotoLinkURL = true;
                viewModel.PreviousPhotoLinkURL = this.GetURL(c => c.GetPersonPhoto(personId, previousId.Value));
            }
            if (nextId.HasValue)
            {
                viewModel.HasNextPhotoLinkURL = true;
                viewModel.NextPhotoLinkURL = this.GetURL(c => c.GetPersonPhoto(personId, nextId.Value));
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
            person.Photo = DatabaseSession.Load<Photo>(Photo.NoPic);
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
                PhotoTinyURL = this.GetURL<PhotosController>(c => c.GetPhotoTiny(x.Photo.PhotoId)),
                PhotoThumbnailURL = this.GetURL<PhotosController>(c => c.GetPhotoThumbnail(x.Photo.PhotoId)),
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
            var awards = DatabaseSession.Query<AwardType>()
                .ToList()
                .OrderBy(x => x.Name)
                .Select(x => new
                {
                    Id = x.AwardTypeId,
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
            var award = new Award(showId.HasValue ? DatabaseSession.Load<Show>(showId.Value) : null, DatabaseSession.Load<Person>(personId), DatabaseSession.Load<AwardType>(awardId), year);
            DatabaseSession.Save(award);
            DatabaseSession.CommitTransaction();
            return this.RedirectToAction(x => x.PersonDetails(personId));
        }

        [HttpPost, Route("{personId}/DeleteAward")]
        public ActionResult DeleteAward(int personId, int awardId)
        {
            var award = DatabaseSession.Get<Award>(awardId);
            DatabaseSession.Delete(award);
            DatabaseSession.CommitTransaction();
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