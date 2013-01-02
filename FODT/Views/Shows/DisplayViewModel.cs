using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FODT.Models.IMDT;

namespace FODT.Views.Shows
{
    public class DisplayViewModel
    {
        public int ShowId { get; set; }
        public string Name { get; set; }
        public string Author { get; set; }
        public Quarter Quarter { get; set; }
        public short Year { get; set; }
        public IEnumerable<Tuple<int, string, short>> OtherPerformances { get; set; }

        public IEnumerable<ClubPosition> ClubPositions { get; set; }

        public class ClubPosition
        {
            public short Year { get; set; }
            public int ClubPositionId { get; set; }
            public string Name { get; set; }
        }

        public IEnumerable<Award> Awards { get; set; }

        public class Award
        {
            public short Year { get; set; }
            public int AwardId { get; set; }
            public string Name { get; set; }
            public int? PersonId { get; set; }
            public string PersonName { get; set; }
            public string PersonLastName { get; set; }
        }

        public IEnumerable<CastRole> Cast { get; set; }

        public class CastRole
        {
            public int PersonId { get; set; }
            public string PersonName { get; set; }
            public string PersonLastName { get; set; }
            public string Role { get; set; }
        }

        public IEnumerable<CrewPosition> Crew { get; set; }

        public class CrewPosition
        {
            public int PersonId { get; set; }
            public string PersonName { get; set; }
            public string PersonLastName { get; set; }
            public string Name { get; set; }
            public int CrewPositionId { get; set; }
        }


    }
}