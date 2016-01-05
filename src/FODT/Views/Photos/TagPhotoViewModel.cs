using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FODT.Views.Photos
{
    public class TagPhotoViewModel
    {
        public string POSTUrl { get; set; }

        public List<KeyValuePair<int, string>> People { get; set; }
        public List<KeyValuePair<int, string>> Shows { get; set; }
    }
}