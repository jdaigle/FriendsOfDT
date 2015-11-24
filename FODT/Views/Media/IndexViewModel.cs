using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FODT.Views.Media
{
    public class IndexViewModel
    {
        public List<Media> RecentlyUploaded { get; set; }
        public List<Media> RandomPic { get; set; }

        public string MediaUploadLinkURL { get; set; }

        public class Media
        {
            public string MediaLinkURL { get; set; }
            public string MediaThumbnailURL { get; set; }
        }
    }
}