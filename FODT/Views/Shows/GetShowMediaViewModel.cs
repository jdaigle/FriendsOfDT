using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FODT.Models;
using FODT.Views.Shared;

namespace FODT.Views.Shows
{
    public class GetShowMediaViewModel
    {
        public int ShowId { get; set; }
        public string ShowTitle { get; set; }
        public short ShowYear { get; set; }
        public int MediaItemId { get; set; }

        public int? PreviousId { get; set; }
        public int? NextId { get; set; }

        public MediaItemViewModel MediaItemViewModel { get; set; }
    }
}