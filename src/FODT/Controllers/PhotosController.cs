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

namespace FODT.Controllers
{
    [RoutePrefix("archive/Photos")]
    public class PhotosController : BaseController
    {
        private static readonly string azureStorageAccountName;
        private static readonly string azureStorageAccountKey;
        private static readonly string azureStorageBaseURL;

        static PhotosController()
        {
            azureStorageAccountName = ConfigurationManager.AppSettings["azure-storage-account-name"];
            azureStorageAccountKey = ConfigurationManager.AppSettings["azure-storage-account-key"];
            azureStorageBaseURL = "https://" + azureStorageAccountName + ".blob.core.windows.net/" + ConfigurationManager.AppSettings["azure-storage-blob-container"] + "/";
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

            AzureBlogStorageUtil.PutBlob(azureStorageBaseURL + photo.GetOriginalFileName()
                , azureStorageAccountName, azureStorageAccountKey
                , original_buffer, "image/jpeg");

            using (var fullSize = ImageUtilities.LoadBitmap(original_buffer))
            {
                using (var thumbnail = ImageUtilities.Resize(fullSize, 240, 240))
                {
                    AzureBlogStorageUtil.PutBlob(azureStorageBaseURL + photo.GetThumbnailFileName()
                        , azureStorageAccountName, azureStorageAccountKey
                        , ImageUtilities.GetBytes(thumbnail, System.Drawing.Imaging.ImageFormat.Jpeg), "image/jpeg");
                }
                using (var tiny = ImageUtilities.Resize(fullSize, 50, 50))
                {
                    AzureBlogStorageUtil.PutBlob(azureStorageBaseURL + photo.GetTinyFileName()
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
                ? this.RedirectToAction<ShowController>(x => x.GetShowPhoto(showPhoto.Show.ShowId, showPhoto.Photo.PhotoId))
                : this.RedirectToAction<PersonController>(x => x.GetPersonPhoto(personPhoto.Person.PersonId, personPhoto.Photo.PhotoId));
        }

        public class UploadPOSTParameters
        {
            public int? PersonId { get; set; }
            public int? ShowId { get; set; }
            public HttpPostedFileBase UploadedFile { get; set; }
        }

        [HttpPost, Route("{id}/tag")]
        public ActionResult Tag(int id, int? personId, int? showId)
        {
            if (personId.HasValue)
            {
                var personPhoto = new PersonPhoto();
                personPhoto.Person = DatabaseSession.Load<Person>(personId.Value);
                personPhoto.Photo = DatabaseSession.Load<Photo>(id);
                personPhoto.InsertedDateTime = DateTime.UtcNow;
                DatabaseSession.Save(personPhoto);
            }
            if (showId.HasValue)
            {
                var showPhoto = new ShowPhoto();
                showPhoto.Show = DatabaseSession.Load<Show>(showId.Value);
                showPhoto.Photo = DatabaseSession.Load<Photo>(id);
                showPhoto.InsertedDateTime = DateTime.UtcNow;
                DatabaseSession.Save(showPhoto);
            }
            DatabaseSession.CommitTransaction();

            if (Request.UrlReferrer == null ||
                string.IsNullOrWhiteSpace(Request.UrlReferrer.PathAndQuery))
            {
                return this.RedirectToAction(x => x.GetPhotoDetail(id));
            }

            return Redirect(Request.UrlReferrer.PathAndQuery);
        }

        [HttpGet, Route("{id}")]
        public ActionResult GetPhotoOriginal(int id)
        {
            var photo = DatabaseSession.Get<Photo>(id);
            if (photo == null)
            {
                return new HttpNotFoundResult();
            }
            return new RedirectResult(azureStorageBaseURL + photo.GetOriginalFileName());
        }

        [HttpGet, Route("{id}/tiny")]
        public ActionResult GetPhotoTiny(int id)
        {
            var photo = DatabaseSession.Get<Photo>(id);
            if (photo == null)
            {
                return new HttpNotFoundResult();
            }
            return new RedirectResult(azureStorageBaseURL + photo.GetTinyFileName());
        }

        [HttpGet, Route("{id}/thumbnail")]
        public ActionResult GetPhotoThumbnail(int id)
        {
            var photo = DatabaseSession.Get<Photo>(id);
            if (photo == null)
            {
                return new HttpNotFoundResult();
            }
            return new RedirectResult(azureStorageBaseURL + photo.GetThumbnailFileName());
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

            viewModel.PhotoViewModel = new PhotoViewModel(DatabaseSession.Get<Photo>(id), DatabaseSession, Url);
            return View("PhotoDetail", viewModel);
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
                .Select(x => new PhotoItemDto
                {
                    PhotoId = x.PhotoId,
                    InsertedDateTime = x.InsertedDateTime,
                })
                .ToList();

            var recentlyUploaded = photos.OrderByDescending(x => x.InsertedDateTime).ThenByDescending(x => x.PhotoId).Take(10).ToList();
            var ran = new Random();
            var randomSet = new HashSet<int>();
            var randomList = new List<PhotoItemDto>(photos);
            while (randomSet.Count < 10)
            {
                var nextIndex = ran.Next(randomList.Count);
                var item = randomList[nextIndex];
                randomList.Remove(item);
                randomSet.Add(item.PhotoId);
            }

            var viewModel = new IndexViewModel();
            viewModel.RecentlyUploaded = recentlyUploaded.Select(x => new IndexViewModel.Photo
            {
                PhotoLinkURL = this.GetURL(c => c.GetPhotoDetail(x.PhotoId)),
                PhotoThumbnailURL = this.GetURL(c => c.GetPhotoThumbnail(x.PhotoId)),
            }).ToList();
            viewModel.RandomPic = randomSet.Select(x => new IndexViewModel.Photo
            {
                PhotoLinkURL = this.GetURL(c => c.GetPhotoDetail(x)),
                PhotoThumbnailURL = this.GetURL(c => c.GetPhotoThumbnail(x)),
            }).ToList();
            return View(viewModel);
        }
    }
}