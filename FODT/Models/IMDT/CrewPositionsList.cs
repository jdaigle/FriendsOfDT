using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FODT.Models.IMDT
{
    public class CrewPositionsList
    {
        public const string ID = "CrewPositionsList";

        private Dictionary<int, CrewPositionListItem> dictionary;
        public CrewPositionListItem this[int id]
        {
            get
            {
                if (dictionary == null)
                {
                    dictionary = CrewPositions.ToDictionary(x => x.Key);
                }
                if (dictionary.ContainsKey(id))
                {
                    return dictionary[id];
                }
                return null;
            }
        }

        public CrewPositionsList()
        {
            this.CrewPositions = new List<CrewPositionListItem>();
        }

        public List<CrewPositionListItem> CrewPositions { get; set; }
    }
}