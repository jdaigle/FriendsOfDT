using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FODT.Views.Show
{
    public class ListShowPhotosViewModel
    {
        public string ShowTitle { get; set; }
        public short ShowYear { get; set; }
        public List<Photo> Photos { get; set; }

        public string ShowLinkURL { get; set; }
        public string PhotoUploadLinkURL { get; set; }

        public class Photo
        {
            public string PhotoLinkURL { get; set; }
            public string PhotoThumbnailURL { get; set; }
        }
    }
}