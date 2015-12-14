using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FODT.Models;

namespace FODT.Views.Awards
{
    public class ByYearViewModel
    {
        public short Year { get; set; }
        public string PreviousYearURL { get; set; }
        public string NextYearURL { get; set; }

        public AwardsTableViewModel AwardsTable { get; internal set; }
    }
}