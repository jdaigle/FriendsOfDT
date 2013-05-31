using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FODT.Views.Show
{
    public class ListShowMediaViewModel
    {
        public int ShowId { get; set; }
        public string ShowTitle { get; set; }
        public short ShowYear { get; set; }
        public List<Media> RelatedMedia { get; set; }

        public class Media
        {
            public int MediaItemId { get; set; }
        }
    }
}