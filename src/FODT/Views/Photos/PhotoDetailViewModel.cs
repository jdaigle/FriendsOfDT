using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FODT.Models;

namespace FODT.Views.Photos
{
    public class PhotoDetailViewModel
    {
        public string PhotoUploadLinkURL { get; set; }
        public bool HasPreviousPhotoLinkURL { get; set; }
        public string PreviousPhotoLinkURL { get; set; }
        public bool HasNextPhotoLinkURL { get; set; }
        public string NextPhotoLinkURL { get; set; }

        public PhotoViewModel PhotoViewModel { get; set; }
    }
}