using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FODT.Models;
using FODT.Views.Shared;

namespace FODT.Views.Media
{
    public class GetItemDetailViewModel
    {
        public string MediaUploadLinkURL { get; set; }
        public bool HasPreviousMediaLinkURL { get; set; }
        public string PreviousMediaLinkURL { get; set; }
        public bool HasNextMediaLinkURL { get; set; }
        public string NextMediaLinkURL { get; set; }

        public MediaItemViewModel MediaItemViewModel { get; set; }
    }
}