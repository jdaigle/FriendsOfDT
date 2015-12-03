using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FODT.Views.Show
{
    public class ListShowMediaViewModel
    {
        public string ShowTitle { get; set; }
        public short ShowYear { get; set; }
        public List<Media> RelatedMedia { get; set; }

        public string ShowLinkURL { get; set; }
        public string MediaUploadLinkURL { get; set; }

        public class Media
        {
            public string MediaTinyURL { get; set; }
            public string MediaThumbnailURL { get; set; }
        }
    }
}