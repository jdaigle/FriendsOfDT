using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FODT.Views.Person
{
    public class ListPersonMediaViewModel
    {
        public int PersonId { get; set; }
        public string PersonFullname { get; set; }
        public List<Media> RelatedMedia { get; set; }

        public class Media
        {
            public int MediaItemId { get; set; }
        }
    }
}