using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FODT.Views.Person
{
    public class ListPersonPhotosViewModel
    {
        public string PersonFullname { get; set; }
        public List<Photo> Photos { get; set; }

        public string PersonLinkURL { get; set; }
        public string PhotoUploadLinkURL { get; set; }

        public class Photo
        {
            public string PhotoLinkURL { get; set; }
            public string PhotoThumbnailURL { get; set; }
        }
    }
}