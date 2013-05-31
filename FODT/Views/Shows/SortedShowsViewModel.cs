using System.Collections.Generic;
using FODT.Models;

namespace FODT.Views.Shows
{
    public class SortedShowsViewModel
    {
        public List<Show> Shows { get; set; }

        public class Show
        {
            public int ShowId { get; set; }
            public string ShowTitle { get; set; }
            public Quarter ShowQuarter { get; set; }
            public short ShowYear { get; set; }
        }
    }
}