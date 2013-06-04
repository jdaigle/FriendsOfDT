using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FODT.Models;
using FODT.Models.IMDT;
using NHibernate;
using NHibernate.Linq;

namespace FODT.Views.Shared
{
    public class MediaItemViewModel
    {
        public int Id { get; set; }

        public List<RelatedShow> RelatedShows { get; set; }
        public List<RelatedPerson> RelatedPeople { get; set; }

        public class RelatedShow
        {
            public int ShowId { get; set; }
            public string ShowTitle { get; set; }
            public Quarter ShowQuarter { get; set; }
            public short ShowYear { get; set; }
        }

        public class RelatedPerson
        {
            public int PersonId { get; set; }
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

        public void PopulateFromDatabase(ISession databaseSession, int mediaItemId)
        {
            var relatedPeople = databaseSession.Query<PersonMedia>().Where(x => x.MediaItem == databaseSession.Load<MediaItem>(mediaItemId)).Fetch(x => x.Person).ToList();
            var relatedshows = databaseSession.Query<ShowMedia>().Where(x => x.MediaItem == databaseSession.Load<MediaItem>(mediaItemId)).Fetch(x => x.Show).ToList();

            this.Id = mediaItemId;
            this.RelatedShows = relatedshows.Select(x => new MediaItemViewModel.RelatedShow
            {
                ShowId = x.Show.ShowId,
                ShowQuarter = x.Show.Quarter,
                ShowYear = x.Show.Year,
                ShowTitle = x.Show.Title,
            }).ToList();
            this.RelatedPeople = relatedPeople.Select(x => new MediaItemViewModel.RelatedPerson
            {
                PersonId = x.Person.PersonId,
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