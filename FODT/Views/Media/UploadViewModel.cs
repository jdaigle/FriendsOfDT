using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FODT.Models;

namespace FODT.Views.Media
{
    public class UploadViewModel
    {
        public List<Show> Shows { get; set; }
        public List<Person> People { get; set; }

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
    }
}