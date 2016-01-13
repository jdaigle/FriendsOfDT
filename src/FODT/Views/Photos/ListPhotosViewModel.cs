using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FODT.Views.Photos
{
    public class ListPhotosViewModel
    {
        public List<Photo> RecentlyUploaded { get; set; }
        public List<Photo> RandomPic { get; set; }

        public string PhotoUploadLinkURL { get; set; }

        public class Photo
        {
            public string PhotoLinkURL { get; set; }
            public string PhotoThumbnailURL { get; set; }
        }
    }
}