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
            this.ClubPositions = new List<ClubPosition>();
            this.Awards = new List<PersonAward>();
        }

        public int Id { get; set; }
        public string FullName { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Honorific { get; set; }
        public string Suffix { get; set; }
        public string AlternateName { get; set; }
        public string EmailAddress { get; set; }
        public string Biography { get; set; }
        public string PictureMediaId { get; set; }
        public List<ClubPosition> ClubPositions { get; set; }
        public List<PersonAward> Awards { get; set; }

        public void SetFullName()
        {
            FullName = this.Honorific + " " + this.FirstName + " " + this.MiddleName + " " + this.LastName + " " + this.Suffix;
            while (FullName.Contains("  "))
            {
                FullName = FullName.Replace("  ", " ");
            }
            FullName = FullName.Trim();
        }
    }
}