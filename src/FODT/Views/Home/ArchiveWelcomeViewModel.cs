using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FODT.Views.Home
{
    public class ArchiveWelcomeViewModel
    {
        public int CastCount { get; internal set; }
        public int CrewCount { get; internal set; }
        public int PersonCount { get; internal set; }
        public int PhotoCount { get; internal set; }
        public int ShowCount { get; internal set; }


        public List<NewShowViewModel> NewShows { get; set; }

        public class NewShowViewModel
        {
            public string Name { get; set; }
            public string Year { get; set; }
            public string ImageUrl { get; set; }
            public string LinkUrl { get; set; }
        }

        public List<NewPersonViewModel> NewPeople { get; set; }

        public class NewPersonViewModel
        {
            public string Name { get; set; }
            public string ImageUrl { get; set; }
            public string LinkUrl { get; set; }
        }

        public List<NewPhotoViewModel> NewPhotos { get; set; }

        public class NewPhotoViewModel
        {
            public string ImageUrl { get; set; }
            public string LinkUrl { get; set; }
        }
    }
}