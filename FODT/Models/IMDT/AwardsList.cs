using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FODT.Models.IMDT
{
    public class AwardsList
    {
        public const string ID = "AwardsList";

        public AwardsList()
        {
            this.Awards = new List<KeyValuePair<int,string>>();
        }

        public List<KeyValuePair<int, string>> Awards { get; set; }

        private Dictionary<int, KeyValuePair<int, string>> dictionary;
        public string this[int id]
        {
            get
            {
                if (dictionary == null)
                {
                    dictionary = Awards.ToDictionary(x => x.Key);
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