using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FODT.Models.IMDT;

namespace FODT.Views.People
{
    public class DisplayViewModel
    {
        public int PersonId { get; set; }
        public string Name { get; set; }
        public string Biography { get; set; }

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
            public int? ShowId { get; set; }
            public string ShowName { get; set; }
            public short? ShowYear { get; set; }
        }

        public IEnumerable<CastRole> CastRoles { get; set; }

        public class CastRole
        {
            public int ShowId { get; set; }
            public string ShowName { get; set; }
            public Quarter ShowQuarter { get; set; }
            public short ShowYear { get; set; }
            public string Role { get; set; }
        }

        public IEnumerable<CrewPosition> CrewPositions { get; set; }

        public class CrewPosition
        {
            public int ShowId { get; set; }
            public string ShowName { get; set; }
            public Quarter ShowQuarter { get; set; }
            public short ShowYear { get; set; }
            public string Name { get; set; }
            public int CrewPositionId { get; set; }
        }
    }
}