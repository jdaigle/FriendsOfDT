using System;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using FODT.Models.IMDT;
using FODT.Security;
using FODT.Views.Photos;
using Microsoft.Web.Mvc;
using NHibernate.Linq;

namespace FODT.Controllers
{
    [RoutePrefix("archive/Show")]
    public class ShowPhotosController : BaseController
    {
        [HttpGet]
        [Route("{showId}/Photos/{photoId?}")]
        public ActionResult ListShowPhotos(int showId, int? photoId = null)
        {
            var show = DatabaseSession.Get<Show>(showId);

            if (photoId == null && show.Photo != null && !show.Photo.IsDefaultNoPic())
            {
                return Redirect(this.GetURL(c => c.ListShowPhotos(showId, show.Photo.PhotoId)));
            }

            var photos = DatabaseSession.Query<ShowPhoto>()
                .Where(x => x.Show == show).Fetch(x => x.Photo)
                .ToList();

            if (photoId == null && photos.Any())
            {
                return Redirect(this.GetURL(c => c.ListShowPhotos(showId, photos.First().Photo.PhotoId)));
            }

            var viewModel = new PhotoListViewModel();
            viewModel.ParentName = show.DisplayTitleWithYear;
            viewModel.ParentLinkURL = this.GetURL<ShowController>(c => c.ShowDetails(showId));
            viewModel.ShowPhotoUploadControl = User.IsInRole(RoleNames.Contributor) || User.IsInRole(RoleNames.Archivist);
            viewModel.PhotoUploadLinkURL = Url.GetUrl(Upload, showId);
            viewModel.Photos = photos
                .OrderBy(x => x.Photo.InsertedDateTime).ThenBy(x => x.Photo.PhotoId)
                .Select(x => new PhotoListViewModel.Photo
                {
                    PhotoLinkURL = this.GetURL(c => c.ListShowPhotos(showId, x.Photo.PhotoId)),
                    PhotoThumbnailURL = x.Photo.GetThumbnailFileURL(),
                }).ToList();

            if (photoId.HasValue)
            {
                var photo = photos.SingleOrDefault(x => x.Photo.PhotoId == photoId.Value);
                if (photo == null)
                {
                    return Redirect(this.GetURL(c => c.ListShowPhotos(showId, null)));
                }
                viewModel.CurrentPhotoViewModel = new PhotoViewModel(photo.Photo, this.GetURL(c => c.ListShowPhotos(showId, photoId.Value)), this);
            }

            return View(viewModel);
        }

        [HttpGet]
        [Route("{showId}/Photos/Upload")]
        public ActionResult Upload(int showId)
        {
            var show = DatabaseSession.Get<Show>(showId);

            var viewModel = new UploadViewModel();
            viewModel.ParentName = show.DisplayTitleWithYear;
            viewModel.ParentLinkURL = this.GetURL<ShowController>(c => c.ShowDetails(showId));
            viewModel.PhotoCount = DatabaseSession.Query<ShowPhoto>().Where(x => x.Show == show).Count();
            viewModel.PhotoListLinkURL = Url.GetUrl(ListShowPhotos, showId, (int?)null);
            viewModel.UploadFormURL = Url.GetUrl(Upload, showId, (PhotosController.UploadPOSTParameters)null);

            return new ViewModelResult(viewModel);
        }

        [HttpPost]
        [Route("{showId}/Photos/Upload")]
        public ActionResult Upload(int showId, PhotosController.UploadPOSTParameters param)
        {
            var validationResult = param.Validate();
            if (validationResult != null)
            {
                return validationResult;
            }

            var photo = PhotosController.Upload(this, param);

            var showPhoto = new ShowPhoto();
            showPhoto.Show = DatabaseSession.Load<Show>(showId);
            showPhoto.Photo = photo;
            showPhoto.InsertedDateTime = DateTime.UtcNow;
            DatabaseSession.Save(showPhoto);

            return Redirect(Url.GetUrl(ListShowPhotos, showId, (int?)photo.PhotoId));
        }

        [HttpPost]
        [Route("{showId}/Photos/{photoId}/delete")]
        public ActionResult DeletePhoto(int showId, int photoId)
        {
            var show = DatabaseSession.Get<Show>(showId);
            var result = PhotosController.Delete(this, photoId);
            if (DatabaseSession.Transaction.IsActive)
            {
                // reassign photo if we just deleted it
                if (show.Photo == null || show.Photo.PhotoId == photoId || show.Photo.IsDefaultNoPic())
                {
                    show.Photo = DatabaseSession.Query<ShowPhoto>()
                        .Where(x => x.Show == show && x.Photo.PhotoId != photoId)
                        .Take(1)
                        .ToList()
                        .Select(x => x.Photo)
                        .FirstOrDefault()
                        ?? DatabaseSession.Load<Photo>(Photo.NoPic);
                }
            }
            if (result != null)
            {
                return result;
            }

            return new ViewModelResult(new HttpApiResult
            {
                HttpStatusCode = HttpStatusCode.OK,
                Message = "Photo Deleted",
                RedirectToURL = this.GetURL(c => c.ListShowPhotos(showId, null)),
            });
        }

        [HttpGet]
        [AjaxOnly]
        [Route("{currentShowId}/Photos/{photoId}/tag")]
        public ActionResult TagPartial(int currentShowId, int photoId)
        {
            return PhotosController.TagPartial(this, photoId, this.GetURL(c => c.AddTag(currentShowId, photoId, null, null)));
        }

        [HttpPost]
        [Route("{currentShowId}/Photos/{photoId}/tag")]
        public ActionResult AddTag(int currentShowId, int photoId, int? personId = null, int? showId = null)
        {
            var result = PhotosController.Tag(this, photoId, personId: personId, showId: showId);
            if (result != null)
            {
                return result;
            }

            return new ViewModelResult(new HttpApiResult
            {
                HttpStatusCode = HttpStatusCode.OK,
                Message = "Tag Added",
                RedirectToURL = this.GetURL(c => c.ListShowPhotos(currentShowId, photoId)),
            });
        }

        [HttpPost]
        [Route("{currentShowId}/Photos/{photoId}/tag/delete")]
        public ActionResult DeleteTag(int currentShowId, int photoId, int? personId = null, int? showId = null)
        {
            var result = PhotosController.DeleteTag(this, photoId, personId: personId, showId: showId);
            if (result != null)
            {
                return result;
            }

            return new ViewModelResult(new HttpApiResult
            {
                HttpStatusCode = HttpStatusCode.OK,
                Message = "Tag Deleted",
                RedirectToURL = this.GetURL(c => c.ListShowPhotos(currentShowId, photoId)),
            });
        }
    }
}