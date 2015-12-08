using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FODT.Views.Home
{
    public class ArchiveWelcomeViewModel
    {
        public int CastCount { get; internal set; }
        public int CrewCount { get; internal set; }
        public int PersonCount { get; internal set; }
        public int PhotoCount { get; internal set; }
        public int ShowCount { get; internal set; }
    }
}