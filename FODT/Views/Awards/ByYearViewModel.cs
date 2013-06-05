using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FODT.Models;

namespace FODT.Views.Awards
{
    public class ByYearViewModel
    {
        public short Year { get; set; }
        public short? PreviousYear { get; set; }
        public short? NextYear { get; set; }

        public IEnumerable<Award> Awards { get; set; }

        public class Award
        {
            public short Year { get; set; }
            public int AwardId { get; set; }
            public string Name { get; set; }

            public int? PersonId { get; set; }
            public string PersonName { get; set; }
            public string PersonLastName { get; set; }

            public int? ShowId { get; set; }
            public string ShowTitle { get; set; }
            public Quarter ShowQuarter { get; set; }
            public short? ShowYear { get; set; }
        }
    }
}