using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing.Imaging;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FODT.Models;
using FODT.Models.IMDT;
using FODT.Views.Photos;
using NHibernate.Linq;

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

        public static Photo Upload(BaseController controller, UploadPOSTParameters param)
        {
            var photo = new Photo();
            controller.DatabaseSession.Save(photo);
            controller.DatabaseSession.Flush(); // to get the ID

            var unique_id = photo.PhotoId.ToString();

            var original_buffer = new byte[param.Photo.ContentLength];
            param.Photo.InputStream.Read(original_buffer, 0, param.Photo.ContentLength);

            using (var originalBitmap = ImageUtilities.LoadBitmap(original_buffer))
            {
                AzureBlogStorageUtil.PutBlob(photo.GetOriginalFileURL()
                , azureStorageAccountName, azureStorageAccountKey
                , ImageUtilities.GetBytes(originalBitmap, ImageFormat.Jpeg), "image/jpeg");

                if (originalBitmap.Width > 600 || originalBitmap.Height > 800)
                {
                    // original image is too large for general display
                    photo.LargeFileIsSameAsOriginal = false;
                    using (var largeBitmap = ImageUtilities.Resize(originalBitmap, 600, 800))
                    {
                        AzureBlogStorageUtil.PutBlob(photo.GetLargeFileURL()
                            , azureStorageAccountName, azureStorageAccountKey
                            , ImageUtilities.GetBytes(largeBitmap, ImageFormat.Jpeg), "image/jpeg");
                    }
                }

                using (var thumbnailBitmap = ImageUtilities.Resize(originalBitmap, 240, 240))
                {
                    AzureBlogStorageUtil.PutBlob(photo.GetThumbnailFileURL()
                        , azureStorageAccountName, azureStorageAccountKey
                        , ImageUtilities.GetBytes(thumbnailBitmap, ImageFormat.Jpeg), "image/jpeg");
                }
                using (var tinyBitmap = ImageUtilities.Resize(originalBitmap, 50, 50))
                {
                    AzureBlogStorageUtil.PutBlob(photo.GetTinyFileURL()
                        , azureStorageAccountName, azureStorageAccountKey
                        , ImageUtilities.GetBytes(tinyBitmap, ImageFormat.Jpeg), "image/jpeg");
                }
            }

            return photo;
        }

        public class UploadPOSTParameters
        {
            public int? PersonId { get; set; }
            public int? ShowId { get; set; }
            public HttpPostedFileBase Photo { get; set; }

            public ActionResult Validate()
            {
                if (Photo == null || Photo.ContentLength == 0)
                {
                    return new HttpBadRequestResult("Missing File");
                }

                if (Photo.ContentLength > 1024d * 1024d * 10d)
                {
                    return new HttpBadRequestResult("File too big: " + Photo.ContentLength);
                }

                return null;
            }
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

        [HttpGet, Route("{id}"), Route("{id}/large")]
        public ActionResult GetPhotoLarge(int id)
        {
            var photo = DatabaseSession.Get<Photo>(id);
            if (photo == null)
            {
                return new HttpNotFoundResult();
            }
            return new RedirectResult(photo.GetLargeFileURL());
        }

        [HttpGet, Route("{id}/original")]
        public ActionResult GetPhotoOriginal(int id)
        {
            var photo = DatabaseSession.Get<Photo>(id);
            if (photo == null)
            {
                return new HttpNotFoundResult();
            }
            return new RedirectResult(photo.GetOriginalFileURL());
        }

        [HttpGet, Route("{id}/tiny")]
        public ActionResult GetPhotoTiny(int id)
        {
            var photo = DatabaseSession.Get<Photo>(id);
            if (photo == null)
            {
                return new HttpNotFoundResult();
            }
            return new RedirectResult(photo.GetTinyFileURL());
        }

        [HttpGet, Route("{id}/thumbnail")]
        public ActionResult GetPhotoThumbnail(int id)
        {
            var photo = DatabaseSession.Get<Photo>(id);
            if (photo == null)
            {
                return new HttpNotFoundResult();
            }
            return new RedirectResult(photo.GetThumbnailFileURL());
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

            viewModel.PhotoUploadLinkURL = "";
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

            viewModel.PhotoViewModel = new PhotoViewModel(DatabaseSession.Get<Photo>(id), "", this);
            return View(viewModel);
        }

        public class PhotoItemDto
        {
            public int PhotoId { get; set; }
            public DateTime InsertedDateTime { get; set; }
        }

        [HttpGet, Route("")]
        public ActionResult List()
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

            var viewModel = new ListPhotosViewModel();
            viewModel.RecentlyUploaded = recentlyUploaded.Select(x => new ListPhotosViewModel.Photo
            {
                PhotoLinkURL = this.GetURL(c => c.GetPhotoDetail(x.PhotoId)),
                PhotoThumbnailURL = x.GetThumbnailFileURL(),
            }).ToList();
            viewModel.RandomPic = randomSet.Select(x => new ListPhotosViewModel.Photo
            {
                PhotoLinkURL = this.GetURL(c => c.GetPhotoDetail(x.PhotoId)),
                PhotoThumbnailURL = x.GetThumbnailFileURL()
            }).ToList();
            return new ViewModelResult(viewModel);
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