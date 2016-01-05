using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FODT.Controllers;
using FODT.Models;
using FODT.Models.IMDT;
using NHibernate;
using NHibernate.Linq;

namespace FODT.Views.Photos
{
    public class PhotoViewModel
    {
        public string PhotoURL { get; set; }
        public string UploadDate { get; set; }

        public List<RelatedShow> RelatedShows { get; set; }
        public List<RelatedPerson> RelatedPeople { get; set; }

        public bool CanDeletePhoto { get; set; }
        public string DeletePhotoURL { get; set; }

        public bool CanAddTag { get; set; }
        public string AddTagURL { get; set; }

        public class RelatedShow
        {
            public string ShowLinkURL { get; set; }
            public string ShowTitle { get; set; }
            public Quarter ShowQuarter { get; set; }
            public short ShowYear { get; set; }

            public bool CanDelete { get; set; }
            public string DeleteURL { get; set; }
        }

        public class RelatedPerson
        {
            public string PersonLinkURL { get; set; }
            public string PersonFullname { get; set; }
            public string PersonLastName { get; set; }

            public bool CanDelete { get; set; }
            public string DeleteURL { get; set; }
        }

        public PhotoViewModel(Photo photo, ISession databaseSession, UrlHelper url)
        {
            var relatedPeople = databaseSession.Query<PersonPhoto>().Where(x => x.Photo == photo).Fetch(x => x.Person).ToList();
            var relatedshows = databaseSession.Query<ShowPhoto>().Where(x => x.Photo == photo).Fetch(x => x.Show).ToList();

            var uploadDateTime = photo.InsertedDateTime;
            if (uploadDateTime == DateTime.MinValue)
            {
                var insertDateTimes = relatedPeople.Select(x => x.InsertedDateTime).Concat(relatedshows.Select(x => x.InsertedDateTime));
                uploadDateTime = insertDateTimes.Where(x => x > DateTime.MinValue).DefaultIfEmpty(DateTime.MinValue).Min();
            }

            if (uploadDateTime > DateTime.MinValue)
            {
                this.UploadDate = uploadDateTime.ToString("MMMM dd, yyyy", CultureInfo.InvariantCulture);
            } else
            {
                this.UploadDate = "Unknown Date";
            }

            this.PhotoURL = photo.GetURL();

            this.CanDeletePhoto = true;
            this.DeletePhotoURL = url.GetURL<PhotosController>(c => c.Delete(photo.PhotoId));

            this.CanAddTag = true;
            this.AddTagURL = url.GetURL<PhotosController>(c => c.Tag(photo.PhotoId));

            this.RelatedShows = relatedshows.Select(x => new PhotoViewModel.RelatedShow
            {
                ShowLinkURL = url.Action<ShowController>(c => c.ListShowPhotos(x.Show.ShowId, x.Photo.PhotoId)),
                ShowQuarter = x.Show.Quarter,
                ShowYear = x.Show.Year,
                ShowTitle = x.Show.DisplayTitle,
            }).ToList();

            this.RelatedPeople = relatedPeople.Select(x => new PhotoViewModel.RelatedPerson
            {
                PersonLinkURL = url.Action<PersonController>(c => c.ListPersonPhotos(x.Person.PersonId, x.Photo.PhotoId)),
                PersonLastName = x.Person.LastName,
                PersonFullname = x.Person.Fullname,
            }).ToList();
        }
    }
}