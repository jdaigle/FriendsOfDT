using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FODT.Views.Photos;

namespace FODT.Views.Show
{
    public class ShowPhotosViewModel
    {
        public string ShowTitle { get; set; }
        public short ShowYear { get; set; }
        public string ShowLinkURL { get; set; }

        public List<Photo> Photos { get; set; }
        public string PhotoUploadLinkURL { get; set; }

        public PhotoViewModel PhotoViewModel { get; set; }

        public class Photo
        {
            public string PhotoLinkURL { get; set; }
            public string PhotoThumbnailURL { get; set; }
        }
    }
}