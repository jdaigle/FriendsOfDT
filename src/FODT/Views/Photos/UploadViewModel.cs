using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FODT.Models;

namespace FODT.Views.Photos
{
    public class UploadViewModel
    {
        public bool ShowParentHeader { get; set; } = true;
        public string ParentName { get; set; }
        public string ParentLinkURL { get; set; }

        public string PhotoListLinkURL { get; set; }
        public int PhotoCount { get; set; }

        public string UploadFormURL { get; set; }
    }
}