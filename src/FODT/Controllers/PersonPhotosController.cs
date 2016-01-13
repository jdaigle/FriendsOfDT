using System.Linq;
using System.Net;
using System.Web.Mvc;
using FODT.Models.IMDT;
using FODT.Views.Photos;
using Microsoft.Web.Mvc;
using NHibernate.Linq;

namespace FODT.Controllers
{
    [RoutePrefix("archive/Person")]
    public class PersonPhotosController : BaseController
    {
        [HttpGet]
        [Route("{personId}/Photos/{photoId?}")]
        public ActionResult ListPersonPhotos(int personId, int? photoId = null)
        {
            var person = DatabaseSession.Get<Person>(personId);

            if (photoId == null && person.Photo != null && !person.Photo.IsDefaultNoPic())
            {
                return Redirect(this.GetURL(c => c.ListPersonPhotos(personId, person.Photo.PhotoId)));
            }

            var photos = DatabaseSession.Query<PersonPhoto>()
                .Where(x => x.Person == person).Fetch(x => x.Photo)
                .ToList();

            if (photoId == null && photos.Any())
            {
                return Redirect(this.GetURL(c => c.ListPersonPhotos(personId, photos.First().Photo.PhotoId)));
            }

            var viewModel = new PhotoListViewModel();
            viewModel.ParentName = person.Fullname;
            viewModel.ParentLinkURL = this.GetURL<PersonController>(c => c.PersonDetails(personId));
            viewModel.PhotoUploadLinkURL = this.GetURL<PhotosController>(c => c.Upload());
            viewModel.Photos = photos
                .OrderBy(x => x.Photo.InsertedDateTime).ThenBy(x => x.Photo.PhotoId)
                .Select(x => new PhotoListViewModel.Photo
                {
                    PhotoLinkURL = this.GetURL(c => c.ListPersonPhotos(personId, x.Photo.PhotoId)),
                    PhotoThumbnailURL = x.Photo.GetThumbnailURL(),
                }).ToList();

            if (photoId.HasValue)
            {
                var photo = photos.SingleOrDefault(x => x.Photo.PhotoId == photoId.Value);
                if (photo == null)
                {
                    return Redirect(this.GetURL(c => c.ListPersonPhotos(personId, null)));
                }
                viewModel.CurrentPhotoViewModel = new PhotoViewModel(photo.Photo, this.GetURL(c => c.ListPersonPhotos(personId, photoId.Value)), this);
            }

            return View(viewModel);
        }

        [HttpPost]
        [Route("{personId}/Photos/{photoId}/delete")]
        public ActionResult DeletePhoto(int personId, int photoId)
        {
            var person = DatabaseSession.Get<Person>(personId);
            var result = PhotosController.Delete(this, photoId);
            if (DatabaseSession.Transaction.IsActive)
            {
                // reassign photo if we just deleted it
                if (person.Photo == null || person.Photo.PhotoId == photoId || person.Photo.IsDefaultNoPic())
                {
                    person.Photo = DatabaseSession.Query<PersonPhoto>()
                        .Where(x => x.Person == person && x.Photo.PhotoId != photoId)
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
                RedirectToURL = this.GetURL(c => c.ListPersonPhotos(personId, null)),
            });
        }

        [HttpGet]
        [AjaxOnly]
        [Route("{currentPersonId}/Photos/{photoId}/tag")]
        public ActionResult TagPartial(int currentPersonId, int photoId)
        {
            return PhotosController.TagPartial(this, photoId, this.GetURL(c => c.AddTag(currentPersonId, photoId, null, null)));
        }

        [HttpPost]
        [Route("{currentPersonId}/Photos/{photoId}/tag")]
        public ActionResult AddTag(int currentPersonId, int photoId, int? personId = null, int? showId = null)
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
                RedirectToURL = this.GetURL(c => c.ListPersonPhotos(currentPersonId, photoId)),
            });
        }

        [HttpPost]
        [Route("{currentPersonId}/Photos/{photoId}/tag/delete")]
        public ActionResult DeleteTag(int currentPersonId, int photoId, int? personId = null, int? showId = null)
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
                RedirectToURL = this.GetURL(c => c.ListPersonPhotos(currentPersonId, photoId)),
            });
        }
    }
}