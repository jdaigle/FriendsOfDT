using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;
using FODT.Controllers;
using FODT.Models;
using FODT.Models.IMDT;
using FODT.Security;
using NHibernate;
using NHibernate.Linq;

namespace FODT.Views.Photos
{
    public class PhotoViewModel
    {
        public string PhotoURL { get; set; }
        public string OriginalPhotoURL { get; set; }
        public string UploadDate { get; set; }

        public List<RelatedShow> RelatedShows { get; set; }
        public List<RelatedPerson> RelatedPeople { get; set; }

        public bool ShowDeletePhotoControl { get; set; }
        public string DeletePhotoURL { get; set; }

        public bool ShowAddTagControl { get; set; }
        public string AddTagPartialURL { get; set; }

        public class RelatedShow
        {
            public string ShowLinkURL { get; set; }
            public string ShowTitle { get; set; }
            public Quarter ShowQuarter { get; set; }
            public short ShowYear { get; set; }

            public bool ShowDeleteControl { get; set; }
            public string DeleteURL { get; set; }
        }

        public class RelatedPerson
        {
            public string PersonLinkURL { get; set; }
            public string PersonFullname { get; set; }
            public string PersonLastName { get; set; }

            public bool ShowDeleteControl { get; set; }
            public string DeleteURL { get; set; }
        }

        public MvcHtmlString Join<T>(IEnumerable<T> collection, Func<T, HelperResult> template)
        {
            var sb = new StringBuilder();
            for (int i = 0; i < collection.Count(); i++)
            {
                sb.Append(template.Invoke(collection.ElementAt(i)));
                if (i != collection.Count() - 1)
                {
                    sb.Append(", ");
                }
            }
            return MvcHtmlString.Create(sb.ToString());
        }

        public PhotoViewModel(Photo photo, string basePhotoURL, BaseController controller)
        {
            var canEditPhoto = controller.ControllerContext.CanEditPhoto(photo);

            var relatedPeople = controller.DatabaseSession.Query<PersonPhoto>().Where(x => x.Photo == photo).Fetch(x => x.Person).ToList();
            var relatedshows = controller.DatabaseSession.Query<ShowPhoto>().Where(x => x.Photo == photo).Fetch(x => x.Show).ToList();

            var uploadDateTime = photo.InsertedDateTime;
            if (uploadDateTime == DateTime.MinValue)
            {
                var insertDateTimes = relatedPeople.Select(x => x.InsertedDateTime).Concat(relatedshows.Select(x => x.InsertedDateTime));
                uploadDateTime = insertDateTimes.Where(x => x > DateTime.MinValue).DefaultIfEmpty(DateTime.MinValue).Min();
            }

            if (uploadDateTime > DateTime.MinValue)
            {
                UploadDate = uploadDateTime.ToString("MMMM dd, yyyy", CultureInfo.InvariantCulture);
            } else
            {
                UploadDate = "Unknown Date";
            }

            PhotoURL = photo.GetLargeFileURL();
            OriginalPhotoURL = photo.GetOriginalFileURL();

            ShowDeletePhotoControl = canEditPhoto;
            DeletePhotoURL = basePhotoURL + "/delete";

            ShowAddTagControl = canEditPhoto;
            AddTagPartialURL = basePhotoURL + "/tag";

            this.RelatedShows = relatedshows.Select(x => new PhotoViewModel.RelatedShow
            {
                ShowLinkURL = controller.Url.Action<ShowPhotosController>(c => c.ListShowPhotos(x.Show.ShowId, x.Photo.PhotoId)),
                ShowQuarter = x.Show.Quarter,
                ShowYear = x.Show.Year,
                ShowTitle = x.Show.DisplayTitle,

                ShowDeleteControl = canEditPhoto && x.Photo != x.Show.Photo,
                DeleteURL = basePhotoURL + $"/tag/delete?showId={x.Show.ShowId}",
            }).ToList();

            this.RelatedPeople = relatedPeople.Select(x => new PhotoViewModel.RelatedPerson
            {
                PersonLinkURL = controller.Url.Action<PersonPhotosController>(c => c.ListPersonPhotos(x.Person.PersonId, x.Photo.PhotoId)),
                PersonLastName = x.Person.LastName,
                PersonFullname = x.Person.Fullname,

                ShowDeleteControl = canEditPhoto && x.Photo != x.Person.Photo,
                DeleteURL = basePhotoURL + $"/tag/delete?personId={x.Person.PersonId}",
            }).ToList();
        }
    }
}