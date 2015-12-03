﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FODT.Models;
using FODT.Models.IMDT;
using FODT.Views.Media;
using FODT.Views.Shared;
using NHibernate.Linq;
using FODT.Database;
using System.Configuration;

namespace FODT.Controllers
{
    [RoutePrefix("Media")]
    public class MediaController : BaseController
    {
        private static readonly string azureStorageAccountName;
        private static readonly string azureStorageAccountKey;
        private static readonly string azureStorageBaseURL;

        static MediaController()
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
                ShowTitle = x.Title,
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

            var mediaItem = new MediaItem();
            DatabaseSession.Save(mediaItem);
            DatabaseSession.Flush(); // to get the ID

            var unique_id = mediaItem.MediaItemId.ToString();

            var original_buffer = new byte[param.UploadedFile.ContentLength];
            param.UploadedFile.InputStream.Read(original_buffer, 0, param.UploadedFile.ContentLength);

            AzureBlogStorageUtil.PutBlob(azureStorageBaseURL + mediaItem.GetOriginalFileName()
                , azureStorageAccountName, azureStorageAccountKey
                , original_buffer, "image/jpeg");

            using (var fullSize = ImageUtilities.LoadBitmap(original_buffer))
            {
                using (var thumbnail = ImageUtilities.Resize(fullSize, 240, 240))
                {
                    AzureBlogStorageUtil.PutBlob(azureStorageBaseURL + mediaItem.GetThumbnailFileName()
                        , azureStorageAccountName, azureStorageAccountKey
                        , ImageUtilities.GetBytes(thumbnail, System.Drawing.Imaging.ImageFormat.Jpeg), "image/jpeg");
                }
                using (var tiny = ImageUtilities.Resize(fullSize, 50, 50))
                {
                    AzureBlogStorageUtil.PutBlob(azureStorageBaseURL + mediaItem.GetTinyFileName()
                        , azureStorageAccountName, azureStorageAccountKey
                        , ImageUtilities.GetBytes(tiny, System.Drawing.Imaging.ImageFormat.Jpeg), "image/jpeg");
                }
            }

            PersonMedia personMedia = null;
            if (param.PersonId.HasValue)
            {
                personMedia = new PersonMedia();
                personMedia.Person = DatabaseSession.Load<Person>(param.PersonId.Value);
                personMedia.MediaItem = mediaItem;
                personMedia.InsertedDateTime = DateTime.UtcNow;
                DatabaseSession.Save(personMedia);
            }

            ShowMedia showMedia = null;
            if (param.ShowId.HasValue)
            {
                showMedia = new ShowMedia();
                showMedia.Show = DatabaseSession.Load<Show>(param.ShowId.Value);
                showMedia.MediaItem = mediaItem;
                showMedia.InsertedDateTime = DateTime.UtcNow;
                DatabaseSession.Save(showMedia);
            }

            DatabaseSession.CommitTransaction();

            return showMedia != null
                ? this.RedirectToAction<ShowController>(x => x.GetShowMedia(showMedia.Show.ShowId, showMedia.MediaItem.MediaItemId))
                : this.RedirectToAction<PersonController>(x => x.GetPersonMedia(personMedia.Person.PersonId, personMedia.MediaItem.MediaItemId));
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
                var personMedia = new PersonMedia();
                personMedia.Person = DatabaseSession.Load<Person>(personId.Value);
                personMedia.MediaItem = DatabaseSession.Load<MediaItem>(id);
                personMedia.InsertedDateTime = DateTime.UtcNow;
                DatabaseSession.Save(personMedia);
            }
            if (showId.HasValue)
            {
                var showMedia = new ShowMedia();
                showMedia.Show = DatabaseSession.Load<Show>(showId.Value);
                showMedia.MediaItem = DatabaseSession.Load<MediaItem>(id);
                showMedia.InsertedDateTime = DateTime.UtcNow;
                DatabaseSession.Save(showMedia);
            }
            DatabaseSession.CommitTransaction();

            if (Request.UrlReferrer == null ||
                string.IsNullOrWhiteSpace(Request.UrlReferrer.PathAndQuery))
            {
                return this.RedirectToAction(x => x.GetItemDetail(id));
            }

            return Redirect(Request.UrlReferrer.PathAndQuery);
        }

        [HttpGet, Route("{id}")]
        public ActionResult GetItem(int id)
        {
            var mediaItem = DatabaseSession.Get<MediaItem>(id);
            if (mediaItem == null)
            {
                return new HttpNotFoundResult();
            }
            return new RedirectResult(azureStorageBaseURL + mediaItem.GetOriginalFileName());
        }

        [HttpGet, Route("{id}/tiny")]
        public ActionResult GetItemTiny(int id)
        {
            var mediaItem = DatabaseSession.Get<MediaItem>(id);
            if (mediaItem == null)
            {
                return new HttpNotFoundResult();
            }
            return new RedirectResult(azureStorageBaseURL + mediaItem.GetTinyFileName());
        }

        [HttpGet, Route("{id}/thumbnail")]
        public ActionResult GetItemThumbnail(int id)
        {
            var mediaItem = DatabaseSession.Get<MediaItem>(id);
            if (mediaItem == null)
            {
                return new HttpNotFoundResult();
            }
            return new RedirectResult(azureStorageBaseURL + mediaItem.GetThumbnailFileName());
        }

        [HttpGet, Route("{id}/detail")]
        public ActionResult GetItemDetail(int id)
        {
            var mediaItems = DatabaseSession
                .Query<MediaItem>()
                .Select(x => new MediaItemDto
                {
                    MediaItemId = x.MediaItemId,
                    InsertedDateTime = x.InsertedDateTime,
                })
                .ToList().OrderBy(x => x.MediaItemId).ToList();


            var index = mediaItems.IndexOf(mediaItems.Single(x => x.MediaItemId == id));
            var previousId = index > 0 ? mediaItems[index - 1].MediaItemId : (int?)null;
            var nextId = index < mediaItems.Count - 1 ? mediaItems[index + 1].MediaItemId : (int?)null;

            var viewModel = new GetItemDetailViewModel();

            viewModel.MediaUploadLinkURL = this.GetURL(c => c.Upload());
            viewModel.PreviousMediaLinkURL = this.GetURL(c => c.GetItemDetail(id));
            viewModel.NextMediaLinkURL = this.GetURL(c => c.GetItemDetail(id));
            if (previousId.HasValue)
            {
                viewModel.HasPreviousMediaLinkURL = true;
                viewModel.PreviousMediaLinkURL = this.GetURL(c => c.GetItemDetail(previousId.Value));
            }
            if (nextId.HasValue)
            {
                viewModel.HasNextMediaLinkURL = true;
                viewModel.NextMediaLinkURL = this.GetURL(c => c.GetItemDetail(nextId.Value));
            }

            viewModel.MediaItemViewModel = new MediaItemViewModel();
            viewModel.MediaItemViewModel.PopulateFromDatabase(DatabaseSession, Url, id);
            return View(viewModel);
        }

        public class MediaItemDto
        {
            public int MediaItemId { get; set; }
            public DateTime InsertedDateTime { get; set; }
        }

        [HttpGet, Route("")]
        public ActionResult Index()
        {
            var mediaItems = DatabaseSession
                .Query<MediaItem>()
                .Select(x => new MediaItemDto
                {
                    MediaItemId = x.MediaItemId,
                    InsertedDateTime = x.InsertedDateTime,
                })
                .ToList();

            var recentlyUploaded = mediaItems.OrderByDescending(x => x.InsertedDateTime).ThenByDescending(x => x.MediaItemId).Take(10).ToList();
            var ran = new Random();
            var randomSet = new HashSet<int>();
            var randomList = new List<MediaItemDto>(mediaItems);
            while (randomSet.Count < 10)
            {
                var nextIndex = ran.Next(randomList.Count);
                var item = randomList[nextIndex];
                randomList.Remove(item);
                randomSet.Add(item.MediaItemId);
            }

            var viewModel = new IndexViewModel();
            viewModel.RecentlyUploaded = recentlyUploaded.Select(x => new IndexViewModel.Media
            {
                MediaLinkURL = this.GetURL(c => c.GetItemDetail(x.MediaItemId)),
                MediaThumbnailURL = this.GetURL(c => c.GetItemThumbnail(x.MediaItemId)),
            }).ToList();
            viewModel.RandomPic = randomSet.Select(x => new IndexViewModel.Media
            {
                MediaLinkURL = this.GetURL(c => c.GetItemDetail(x)),
                MediaThumbnailURL = this.GetURL(c => c.GetItemThumbnail(x)),
            }).ToList();
            return View(viewModel);
        }
    }
}