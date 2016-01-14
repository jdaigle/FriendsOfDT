using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FODT.Models;
using FODT.Views.Awards;

namespace FODT.Views.Show
{
    public class ShowDetailsViewModel
    {
        public string PhotoLinkURL { get; set; }
        public string PhotoThumbnailURL { get; set; }
        public string PhotoListLinkURL { get; set; }

        public bool ShowPhotoUploadControl { get; set; }
        public string PhotoUploadLinkURL { get; set; }

        public string Title { get; set; }
        public string Author { get; set; }
        public Quarter Quarter { get; set; }
        public short Year { get; set; }
        public string FunFacts { get; set; }
        public string Pictures { get; set; }
        public string Toaster { get; set; }

        public string NextShowLinkURL { get; set; }
        public string PreviousShowLinkURL { get; set; }

        public OtherPerformancesTableViewModel OtherPerformances { get; set; }
        public AwardsTableViewModel AwardsTable { get; set; }
        public CastRolesTableViewModel CastRolesTable { get; set; }
        public CrewPositionsTableViewModel CrewPositionsTable { get; set; }

        public int PhotoCount { get; set; }
        public List<NewPhotoViewModel> NewPhotos { get; set; }

        public class NewPhotoViewModel
        {
            public string PhotoLinkURL { get; set; }
            public string PhotoTinyURL { get; set; }
        }
    }
}