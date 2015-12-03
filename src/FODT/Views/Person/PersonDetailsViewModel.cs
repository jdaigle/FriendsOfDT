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
        public string MediaLinkURL { get; set; } // Person.GetPersonMedia(Model.PersonId, Model.MediaItemId)
        public string MediaThumbnailURL { get; set; } // Media.GetItemThumbnail(Model.MediaItemId)
        public string MediaListLinkURL { get; set; } // Media.ListPersonMedia(Model.PersonId)
        public string MediaUploadLinkURL { get; set; } // 

        public string EditLinkURL { get; set; }

        public string FullName { get; set; }
        public string Biography { get; set; }

        public ClubPositionsTableViewModel ClubPositionsTable { get; set; }
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