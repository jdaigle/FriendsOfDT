using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FODT.Models.IMDT
{
    public class ClubPositionsList
    {
        public const string ID = "ClubPositionsList";

        public ClubPositionsList()
        {
            this.ClubPositions = new List<KeyValuePair<int, string>>();
        }

        public List<KeyValuePair<int, string>> ClubPositions { get; set; }

        private Dictionary<int, KeyValuePair<int, string>> dictionary;
        public string this[int id]
        {
            get
            {
                if (dictionary == null)
                {
                    dictionary = ClubPositions.ToDictionary(x => x.Key);
                }
                if (dictionary.ContainsKey(id))
                {
                    return dictionary[id].Value;
                }
                return string.Empty;
            }
        }

    }
}