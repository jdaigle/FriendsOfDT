using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FODT.Models.IMDT
{
    public class Show
    {
        public Show()
        {
            this.Awards = new List<ShowAward>();
            this.Cast = new List<CastMember>();
            this.Crew = new List<CrewMember>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Author { get; set; }
        public short Year { get; set; }
        public Quarter Quarter { get; set; }
        public List<ShowAward> Awards { get; set; }
        public List<CastMember> Cast { get; set; }
        public List<CrewMember> Crew { get; set; }
    }
}