using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FODT.Models;
using FODT.Models.IMDT;
using FODT.Views.Photos;
using NHibernate.Linq;
using FODT.Database;
using System.Configuration;
using Microsoft.Web.Mvc;
using System.Net;
using NHibernate;

namespace FODT.Controllers
{
    [RoutePrefix("archive/Photos")]
    public class PhotosController : BaseController
    {
        private static readonly string azureStorageAccountName;
        private static readonly string azureStorageAccountKey;

        static PhotosController()
        {
            azureStorageAccountName = ConfigurationManager.AppSettings["azure-storage-account-name"];
            azureStorageAccountKey = ConfigurationManager.AppSettings["azure-storage-account-key"];
        }


        [HttpGet, Route("Upload")]
        public ActionResult Upload()
        {
            var people = DatabaseSession.Query<Person>().ToList();
            var shows = DatabaseSession.Query<Show>().ToList();

            var viewModel = new UploadViewModel();
            viewModel.People = people.Select(x => new UploadViewModel.Person
            {
                PersonId = x.PersonId,
                PersonLastName = x.LastName,
                PersonFirstName = x.FirstName,
                PersonFullname = x.Fullname,
            }).ToList();
            viewModel.Shows = shows.Select(x => new UploadViewModel.Show
            {
                ShowId = x.ShowId,
                ShowQuarter = x.Quarter,
                ShowYear = x.Year,
                ShowTitle = x.DisplayTitle,
            }).ToList();

            return View(viewModel);
        }

        [HttpPost, Route("Upload")]
        public ActionResult Upload(UploadPOSTParameters param)
        {
            if (!param.PersonId.HasValue && !param.ShowId.HasValue)
            {
                throw new InvalidOperationException("PersonId or ShowId must have a value");
            }

            if (param.UploadedFile == null || param.UploadedFile.ContentLength == 0)
            {
                throw new InvalidOperationException("Missing File");
            }

            if (param.UploadedFile.ContentLength > 1024d * 1024d * 1.5d)
            {
                throw new InvalidOperationException("File too big: " + param.UploadedFile.ContentLength);
            }

            var photo = new Photo();
            DatabaseSession.Save(photo);
            DatabaseSession.Flush(); // to get the ID

            var unique_id = photo.PhotoId.ToString();

            var original_buffer = new byte[param.UploadedFile.ContentLength];
            param.UploadedFile.InputStream.Read(original_buffer, 0, param.UploadedFile.ContentLength);

            AzureBlogStorageUtil.PutBlob(photo.GetURL()
                , azureStorageAccountName, azureStorageAccountKey
                , original_buffer, "image/jpeg");

            using (var fullSize = ImageUtilities.LoadBitmap(original_buffer))
            {
                using (var thumbnail = ImageUtilities.Resize(fullSize, 240, 240))
                {
                    AzureBlogStorageUtil.PutBlob(photo.GetThumbnailURL()
                        , azureStorageAccountName, azureStorageAccountKey
                        , ImageUtilities.GetBytes(thumbnail, System.Drawing.Imaging.ImageFormat.Jpeg), "image/jpeg");
                }
                using (var tiny = ImageUtilities.Resize(fullSize, 50, 50))
                {
                    AzureBlogStorageUtil.PutBlob(photo.GetTinyURL()
                        , azureStorageAccountName, azureStorageAccountKey
                        , ImageUtilities.GetBytes(tiny, System.Drawing.Imaging.ImageFormat.Jpeg), "image/jpeg");
                }
            }

            PersonPhoto personPhoto = null;
            if (param.PersonId.HasValue)
            {
                personPhoto = new PersonPhoto();
                personPhoto.Person = DatabaseSession.Load<Person>(param.PersonId.Value);
                personPhoto.Photo = photo;
                personPhoto.InsertedDateTime = DateTime.UtcNow;
                DatabaseSession.Save(personPhoto);
            }

            ShowPhoto showPhoto = null;
            if (param.ShowId.HasValue)
            {
                showPhoto = new ShowPhoto();
                showPhoto.Show = DatabaseSession.Load<Show>(param.ShowId.Value);
                showPhoto.Photo = photo;
                showPhoto.InsertedDateTime = DateTime.UtcNow;
                DatabaseSession.Save(showPhoto);
            }

            DatabaseSession.CommitTransaction();

            return showPhoto != null
                ? this.RedirectToAction<ShowPhotosController>(x => x.ListShowPhotos(showPhoto.Show.ShowId, showPhoto.Photo.PhotoId))
                : this.RedirectToAction<PersonPhotosController>(x => x.ListPersonPhotos(personPhoto.Person.PersonId, personPhoto.Photo.PhotoId));
        }

        public class UploadPOSTParameters
        {
            public int? PersonId { get; set; }
            public int? ShowId { get; set; }
            public HttpPostedFileBase UploadedFile { get; set; }
        }

        public static ActionResult TagPartial(BaseController controller, int id, string postURL)
        {
            if (id == Photo.NoPic)
            {
                controller.RollbackTransactionFast();
                return new HttpBadRequestResult("Cannot tag this photo.");
            }

            var model = new TagPhotoViewModel();
            model.POSTUrl = postURL;
            model.Shows = controller.DatabaseSession.Query<Show>()
             .ToList()
             .OrderBy(x => x, ShowComparer.ReverseChronologicalShowComparer)
             .Select(x => new KeyValuePair<int, string>(x.ShowId, x.Year + " " + x.DisplayTitle))
             .ToList();
            model.People = controller.DatabaseSession.Query<Person>()
             .ToList()
             .OrderBy(x => x.SortableName)
             .Select(x => new KeyValuePair<int, string>(x.PersonId, x.Fullname))
             .ToList();

            return new ViewModelResult(model);
        }

        public static ActionResult Tag(BaseController controller, int id, int? personId = null, int? showId = null)
        {
            if (id == Photo.NoPic)
            {
                controller.RollbackTransactionFast();
                return new HttpBadRequestResult("Cannot tag this photo.");
            }

            if (personId.HasValue && !controller.DatabaseSession.Query<PersonPhoto>().Any(x => x.Photo.PhotoId == id && x.Person.PersonId == personId.Value))
            {
                var personPhoto = new PersonPhoto();
                personPhoto.Person = controller.DatabaseSession.Load<Person>(personId.Value);
                personPhoto.Photo = controller.DatabaseSession.Load<Photo>(id);
                personPhoto.InsertedDateTime = DateTime.UtcNow;
                controller.DatabaseSession.Save(personPhoto);
            }
            if (showId.HasValue && !controller.DatabaseSession.Query<ShowPhoto>().Any(x => x.Photo.PhotoId == id && x.Show.ShowId == showId.Value))
            {
                var showPhoto = new ShowPhoto();
                showPhoto.Show = controller.DatabaseSession.Load<Show>(showId.Value);
                showPhoto.Photo = controller.DatabaseSession.Load<Photo>(id);
                showPhoto.InsertedDateTime = DateTime.UtcNow;
                controller.DatabaseSession.Save(showPhoto);
            }

            return null;
        }

        public static ActionResult DeleteTag(BaseController controller, int id, int? personId = null, int? showId = null)
        {
            if (id == Photo.NoPic)
            {
                controller.RollbackTransactionFast();
                return new HttpBadRequestResult("Cannot tag this photo.");
            }

            if (personId.HasValue)
            {
                // only delete the tag if the photo is not the Person's default photo
                controller.DatabaseSession.Execute(
                    "DELETE FROM PersonPhoto WHERE PhotoId = @PhotoId AND PersonId = @PersonId AND NOT EXISTS (SELECT * FROM Person WHERE PhotoId = @PhotoId AND PersonId = @PersonId)"
                    , new { PhotoId = id, PersonId = personId.Value });
            }

            if (showId.HasValue)
            {
                // only delete the tag if the photo is not the Show's default photo
                controller.DatabaseSession.Execute(
                    "DELETE FROM ShowPhoto WHERE PhotoId = @PhotoId AND ShowId = @ShowId AND NOT EXISTS (SELECT * FROM Show WHERE PhotoId = @PhotoId AND ShowId = @ShowId)"
                    , new { PhotoId = id, ShowId = showId.Value });
            }

            return null;
        }

        [HttpGet, Route("{id}")]
        public ActionResult GetPhotoOriginal(int id)
        {
            var photo = DatabaseSession.Get<Photo>(id);
            if (photo == null)
            {
                return new HttpNotFoundResult();
            }
            return new RedirectResult(photo.GetURL());
        }

        [HttpGet, Route("{id}/tiny")]
        public ActionResult GetPhotoTiny(int id)
        {
            var photo = DatabaseSession.Get<Photo>(id);
            if (photo == null)
            {
                return new HttpNotFoundResult();
            }
            return new RedirectResult(photo.GetTinyURL());
        }

        [HttpGet, Route("{id}/thumbnail")]
        public ActionResult GetPhotoThumbnail(int id)
        {
            var photo = DatabaseSession.Get<Photo>(id);
            if (photo == null)
            {
                return new HttpNotFoundResult();
            }
            return new RedirectResult(photo.GetThumbnailURL());
        }

        [HttpGet, Route("{id}/detail")]
        public ActionResult GetPhotoDetail(int id)
        {
            var photos = DatabaseSession
                .Query<Photo>()
                .Select(x => new PhotoItemDto
                {
                    PhotoId = x.PhotoId,
                    InsertedDateTime = x.InsertedDateTime,
                })
                .ToList().OrderBy(x => x.PhotoId).ToList();


            var index = photos.IndexOf(photos.Single(x => x.PhotoId == id));
            var previousId = index > 0 ? photos[index - 1].PhotoId : (int?)null;
            var nextId = index < photos.Count - 1 ? photos[index + 1].PhotoId : (int?)null;

            var viewModel = new PhotoDetailViewModel();

            viewModel.PhotoUploadLinkURL = this.GetURL(c => c.Upload());
            viewModel.PreviousPhotoLinkURL = this.GetURL(c => c.GetPhotoDetail(id));
            viewModel.NextPhotoLinkURL = this.GetURL(c => c.GetPhotoDetail(id));
            if (previousId.HasValue)
            {
                viewModel.HasPreviousPhotoLinkURL = true;
                viewModel.PreviousPhotoLinkURL = this.GetURL(c => c.GetPhotoDetail(previousId.Value));
            }
            if (nextId.HasValue)
            {
                viewModel.HasNextPhotoLinkURL = true;
                viewModel.NextPhotoLinkURL = this.GetURL(c => c.GetPhotoDetail(nextId.Value));
            }

            viewModel.PhotoViewModel = new PhotoViewModel(DatabaseSession.Get<Photo>(id), "", DatabaseSession, Url);
            return View(viewModel);
        }

        public class PhotoItemDto
        {
            public int PhotoId { get; set; }
            public DateTime InsertedDateTime { get; set; }
        }

        [HttpGet, Route("")]
        public ActionResult Index()
        {
            var photos = DatabaseSession
                .Query<Photo>()
                .ToList();

            var recentlyUploaded = photos.OrderByDescending(x => x.InsertedDateTime).ThenByDescending(x => x.PhotoId).Take(10).ToList();
            var ran = new Random();
            var randomSet = new HashSet<Photo>();
            var randomList = new List<Photo>(photos);
            while (randomSet.Count < 10)
            {
                var nextIndex = ran.Next(randomList.Count);
                var item = randomList[nextIndex];
                randomList.Remove(item);
                randomSet.Add(item);
            }

            var viewModel = new IndexViewModel();
            viewModel.RecentlyUploaded = recentlyUploaded.Select(x => new IndexViewModel.Photo
            {
                PhotoLinkURL = this.GetURL(c => c.GetPhotoDetail(x.PhotoId)),
                PhotoThumbnailURL = x.GetThumbnailURL(),
            }).ToList();
            viewModel.RandomPic = randomSet.Select(x => new IndexViewModel.Photo
            {
                PhotoLinkURL = this.GetURL(c => c.GetPhotoDetail(x.PhotoId)),
                PhotoThumbnailURL = x.GetThumbnailURL()
            }).ToList();
            return View(viewModel);
        }

        public static ActionResult Delete(BaseController controller, int id)
        {
            if (id == Photo.NoPic)
            {
                controller.RollbackTransactionFast();
                return new HttpBadRequestResult("Cannot delete this photo.");
            }

            var photo = controller.DatabaseSession.Get<Photo>(id);
            if (photo == null)
            {
                return new HttpNotFoundResult("Photo not found");
            }

            // This is a hard delete.
            controller.DatabaseSession.Execute(@"
UPDATE Person SET PhotoId = 1 WHERE PhotoId = @PhotoId;
UPDATE Show SET PhotoId = 1 WHERE PhotoId = @PhotoId;
DELETE FROM PersonPhoto WHERE PhotoId = @PhotoId;
DELETE FROM ShowPhoto WHERE PhotoId = @PhotoId;
DELETE FROM Photo WHERE PhotoId = @PhotoId;
", new { PhotoId = id });

            // TODO: delete blob

            return null;
        }
    }
}