using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FriendsOfDT.Models.IMDT
{
    public class Person
    {
        public Person()
        {
            this.AssociatedMedia = new List<string>();
        }

        public PersonInfo PersonInfo { get; set; }
        public List<ClubPosition> ClubPositions { get; set; }
        public List<string> AssociatedMedia { get; set; }
    }
}