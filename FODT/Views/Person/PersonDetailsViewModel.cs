using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FODT.Models;
using FODT.Views.Awards;
using FODT.Views.Shared;

namespace FODT.Views.Person
{
    public class PersonDetailsViewModel
    {
        public bool CanEdit { get; set; }

        public MvcHtmlString RenderEditControls(Func<object, System.Web.WebPages.HelperResult> template)
        {
            if (CanEdit)
            {
                return MvcHtmlString.Create(template.Invoke(null).ToHtmlString());
            }
            else
            {
                return MvcHtmlString.Empty;
            }
        }

        public string MediaLinkURL { get; set; } // Person.GetPersonMedia(Model.PersonId, Model.MediaItemId)
        public string MediaThumbnailURL { get; set; } // Media.GetItemThumbnail(Model.MediaItemId)
        public string MediaListLinkURL { get; set; } // Media.ListPersonMedia(Model.PersonId)
        public string MediaUploadLinkURL { get; set; } // 

        public string EditLinkURL { get; set; }
        public string AddAwardURL { get; set; }
        public string AddClubPositionURL { get; set; }
        public string AddCastURL { get; set; }
        public string AddCrewURL { get; set; }

        public int PersonId { get; set; }
        public string FullName { get; set; }
        public string Biography { get; set; }
        public int MediaItemId { get; set; }

        public IEnumerable<ClubPosition> ClubPositions { get; set; }

        public class ClubPosition
        {
            public short Year { get; set; }
            public string Name { get; set; }
            public int ClubPositionId { get; set; }
            public string DeleteClubPositionURL { get; set; }
        }

        public AwardsTableViewModel AwardsTable { get; set; }

        public IEnumerable<Award> Awards { get; set; }

        public class Award
        {
            public string AwardYearLinkURL { get; set; }
            public string DeleteAwardURL { get; set; }
            public short Year { get; set; }
            public int AwardId { get; set; }
            public string Name { get; set; }
            public string ShowLinkURL { get; set; }
            public int? ShowId { get; set; }
            public string ShowName { get; set; }
            public Quarter ShowQuarter { get; set; }
            public short? ShowYear { get; set; }
        }

        public IEnumerable<CastRole> CastRoles { get; set; }

        public class CastRole
        {
            public string DeleteCastURL { get; set; }
            public string ShowLinkURL { get; set; }
            public int ShowId { get; set; }
            public string ShowName { get; set; }
            public Quarter ShowQuarter { get; set; }
            public short ShowYear { get; set; }
            public string Role { get; set; }
            public int ShowCastId { get; set; }
        }

        public IEnumerable<CrewPosition> CrewPositions { get; set; }

        public class CrewPosition
        {
            public string DeleteCrewURL { get; set; }
            public string ShowLinkURL { get; set; }
            public int ShowId { get; set; }
            public string ShowName { get; set; }
            public Quarter ShowQuarter { get; set; }
            public short ShowYear { get; set; }
            public string Name { get; set; }

            public int ShowCrewId { get; set; }
        }

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