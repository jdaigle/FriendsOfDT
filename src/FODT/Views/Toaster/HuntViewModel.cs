using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FODT.Models;

namespace FODT.Views.Toaster
{
    public class HuntViewModel
    {
        public HuntViewModel()
        {
            Shows = new List<Show>();
        }

        public List<Show> Shows { get; set; }

        public class Show
        {
            public int ShowId { get; set; }
            public string ShowName { get; set; }
            public Quarter ShowQuarter { get; set; }
            public short ShowYear { get; set; }
            public string Toaster { get; set; }
        }
    }
}