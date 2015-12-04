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

        public class RelatedShow
        {
            public string ShowLinkURL { get; set; }
            public string ShowTitle { get; set; }
            public Quarter ShowQuarter { get; set; }
            public short ShowYear { get; set; }
        }

        public class RelatedPerson
        {
            public string PersonLinkURL { get; set; }
            public string PersonFullname { get; set; }
            public string PersonLastName { get; set; }
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

            this.PhotoURL = url.Action<PhotosController>(c => c.GetPhotoOriginal(photo.PhotoId));

            this.RelatedShows = relatedshows.Select(x => new PhotoViewModel.RelatedShow
            {
                ShowLinkURL = url.Action<ShowController>(c => c.ShowDetails(x.Show.ShowId)),
                ShowQuarter = x.Show.Quarter,
                ShowYear = x.Show.Year,
                ShowTitle = x.Show.Title,
            }).ToList();

            this.RelatedPeople = relatedPeople.Select(x => new PhotoViewModel.RelatedPerson
            {
                PersonLinkURL = url.Action<PersonController>(c => c.PersonDetails(x.Person.PersonId)),
                PersonLastName = x.Person.LastName,
                PersonFullname = x.Person.Fullname,
            }).ToList();
        }
    }
}