using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FODT.Views.Person
{
    public class EditPersonViewModel
    {
        public static EditPersonViewModel Empty()
        {
            return new EditPersonViewModel() { RelatedMedia = new List<Media>() };
        }

        public string POSTUrl { get; set; }

        public int? PersonId { get; set; }

        public string Honorific { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Suffix { get; set; }
        public string Nickname { get; set; }
        public string Biography { get; set; }

        public int? DefaultMediaItemId { get; set; }

        public List<Media> RelatedMedia { get; set; }

        public class Media
        {
            public int MediaItemId { get; set; }
            public string MediaTinyURL { get; set; }
            public string MediaThumbnailURL { get; set; }
        }
    }
}