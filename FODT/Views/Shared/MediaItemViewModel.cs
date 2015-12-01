using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FODT.Controllers;
using FODT.Models;
using FODT.Models.IMDT;
using NHibernate;
using NHibernate.Linq;

namespace FODT.Views.Shared
{
    public class MediaItemViewModel
    {
        public int Id { get; set; }
        public string ItemURL { get; set; }
        public string TagPOSTUrl { get; set; }

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

        public List<Show> AllShows { get; set; }
        public List<Person> AllPeople { get; set; }

        public class Show
        {
            public int ShowId { get; set; }
            public string ShowTitle { get; set; }
            public Quarter ShowQuarter { get; set; }
            public short ShowYear { get; set; }
        }

        public class Person
        {
            public int PersonId { get; set; }
            public string PersonFullname { get; set; }
            public string PersonLastName { get; set; }
            public string PersonFirstName { get; set; }
        }

        public void PopulateFromDatabase(ISession databaseSession, UrlHelper url, int mediaItemId)
        {
            var relatedPeople = databaseSession.Query<PersonMedia>().Where(x => x.MediaItem == databaseSession.Load<MediaItem>(mediaItemId)).Fetch(x => x.Person).ToList();
            var relatedshows = databaseSession.Query<ShowMedia>().Where(x => x.MediaItem == databaseSession.Load<MediaItem>(mediaItemId)).Fetch(x => x.Show).ToList();

            this.Id = mediaItemId;
            this.ItemURL = url.Action<MediaController>(c => c.GetItem(mediaItemId));
            this.TagPOSTUrl = url.Action<MediaController>(c => c.Tag(mediaItemId, null, null));
            this.RelatedShows = relatedshows.Select(x => new MediaItemViewModel.RelatedShow
            {
                ShowLinkURL = url.Action<ShowController>(c => c.ShowDetails(x.Show.ShowId)),
                ShowQuarter = x.Show.Quarter,
                ShowYear = x.Show.Year,
                ShowTitle = x.Show.Title,
            }).ToList();
            this.RelatedPeople = relatedPeople.Select(x => new MediaItemViewModel.RelatedPerson
            {
                PersonLinkURL = url.Action<PersonController>(c => c.PersonDetails(x.Person.PersonId)),
                PersonLastName = x.Person.LastName,
                PersonFullname = x.Person.Fullname,
            }).ToList();

            var people = databaseSession.Query<Models.IMDT.Person>().ToList();
            var shows = databaseSession.Query<Models.IMDT.Show>().ToList();

            this.AllPeople = people.Select(x => new MediaItemViewModel.Person
            {
                PersonId = x.PersonId,
                PersonLastName = x.LastName,
                PersonFirstName = x.FirstName,
                PersonFullname = x.Fullname,
            }).ToList();
            this.AllShows = shows.Select(x => new MediaItemViewModel.Show
            {
                ShowId = x.ShowId,
                ShowQuarter = x.Quarter,
                ShowYear = x.Year,
                ShowTitle = x.Title,
            }).ToList();
        }
    }
}