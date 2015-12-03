using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FODT.Models;
using FODT.Models.IMDT;
using NHibernate;
using NHibernate.Linq;

namespace FODT.Views.Home
{
    public class AdminViewModel
    {
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

        public void PopulateFromDatabase(ISession databaseSession)
        {
            var people = databaseSession.Query<Models.IMDT.Person>().ToList();
            var shows = databaseSession.Query<Models.IMDT.Show>().ToList();

            this.AllPeople = people.Select(x => new AdminViewModel.Person
            {
                PersonId = x.PersonId,
                PersonLastName = x.LastName,
                PersonFirstName = x.FirstName,
                PersonFullname = x.Fullname,
            }).ToList();

            this.AllShows = shows.Select(x => new AdminViewModel.Show
            {
                ShowId = x.ShowId,
                ShowQuarter = x.Quarter,
                ShowYear = x.Year,
                ShowTitle = x.Title,
            }).ToList();
        }
    }
}