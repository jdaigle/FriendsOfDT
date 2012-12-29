using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FODT.Models.IMDT
{
    public class Person
    {
        public Person()
        {
            this.AlsoKnownAs = new List<string>();
            this.ClubPositions = new List<ClubPosition>();
            this.Awards = new List<PersonAward>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public List<string> AlsoKnownAs { get; set; }
        public string EmailAddress { get; set; }
        public string Biography { get; set; }
        public string PictureMediaId { get; set; }
        public List<ClubPosition> ClubPositions { get; set; }
        public List<PersonAward> Awards { get; set; }
    }
}