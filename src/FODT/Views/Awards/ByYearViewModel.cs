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
        public string PreviousYearURL { get; set; }
        public string NextYearURL { get; set; }

        public IEnumerable<Award> Awards { get; set; }

        public class Award
        {
            public short Year { get; set; }
            public int AwardId { get; set; }
            public string Name { get; set; }

            public string AwardShowLinkURL { get; set; }
            public string AwardShowLinkText { get; set; }

            public string AwardPersonLinkURL { get; set; }
            public string AwardPersonLinkText { get; set; }
        }
    }
}