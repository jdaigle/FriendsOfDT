using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FODT.Models;
using FODT.Views.Awards;
using FODT.Views.Shared;
using FODT.Views.Show;

namespace FODT.Views.Person
{
    public class PersonDetailsViewModel
    {
        public string PhotoLinkURL { get; set; }
        public string PhotoThumbnailURL { get; set; }
        public string PhotoListLinkURL { get; set; }
        public string PhotoUploadLinkURL { get; set; } // 

        public string EditLinkURL { get; set; }

        public string FullName { get; set; }
        public string Biography { get; set; }

        public ClubPositionsTableViewModel ClubPositionsTable { get; set; }
        public AwardsTableViewModel AwardsTable { get; set; }
        public CastRolesTableViewModel CastRolesTable { get; set; }
        public CrewPositionsTableViewModel CrewPositionsTable { get; set; }

        public int PhotoCount { get; set; }
        public List<NewPhotosViewModel> NewPhotos { get; set; }

        public class NewPhotosViewModel
        {
            public string PhotoLinkURL { get; set; }
            public string PhotoTinyURL { get; set; }
        }
    }
}