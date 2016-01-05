using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FODT.Views.Photos
{
    public class PhotoListViewModel
    {
        public bool ShowParentHeader { get; set; } = true;
        public string ParentName { get; set; }
        public string ParentLinkURL { get; set; }

        public List<Photo> Photos { get; set; }
        public string PhotoUploadLinkURL { get; set; }

        public PhotoViewModel CurrentPhotoViewModel { get; set; }

        public class Photo
        {
            public string PhotoLinkURL { get; set; }
            public string PhotoThumbnailURL { get; set; }
        }
    }
}