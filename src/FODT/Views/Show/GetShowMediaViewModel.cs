using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FODT.Models;
using FODT.Views.Shared;

namespace FODT.Views.Show
{
    public class GetShowMediaViewModel
    {
        public string UploadLinkURL { get; set; }
        public string ShowLinkURL { get; set; }
        public string ShowTitle { get; set; }
        public short ShowYear { get; set; }
        public int MediaItemId { get; set; }

        public string PreviousItemLinkURL { get; set; }
        public string NextItemLinkURL { get; set; }

        public MediaItemViewModel MediaItemViewModel { get; set; }
    }
}