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
        public int Id { get; set; }
        public int? PreviousId { get; set; }
        public int? NextId { get; set; }

        public MediaItemViewModel MediaItemViewModel { get; set; }
    }
}