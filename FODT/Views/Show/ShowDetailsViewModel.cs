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
        public string MediaLinkURL { get; set; } // Person.GetPersonMedia(Model.PersonId, Model.MediaItemId)
        public string MediaThumbnailURL { get; set; } // Media.GetItemThumbnail(Model.MediaItemId)
        public string MediaListLinkURL { get; set; } // Media.ListPersonMedia(Model.PersonId)
        public string MediaUploadLinkURL { get; set; } // 

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

        public int RelatedMediaCount { get; set; }
        public List<RelatedMediaViewModel> NewRelatedMedia { get; set; }

        public class RelatedMediaViewModel
        {
            public int ID { get; set; }
            public string MediaLinkURL { get; set; } // Person.GetPersonMedia(Model.PersonId, Model.MediaItemId)
            public string MediaThumbnailURL { get; set; } // Media.GetItemThumbnail(Model.MediaItemId)
        }
    }
}