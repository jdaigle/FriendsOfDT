using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FODT.Models;

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
    }
}