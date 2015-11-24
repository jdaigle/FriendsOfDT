using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FODT.Views.Person
{
    public class ListPersonMediaViewModel
    {
        public string PersonFullname { get; set; }
        public List<Media> RelatedMedia { get; set; }

        public string PersonLinkURL { get; set; }
        public string MediaUploadLinkURL { get; set; }

        public class Media
        {
            public string MediaLinkURL { get; set; }
            public string MediaThumbnailURL { get; set; }
        }
    }
}