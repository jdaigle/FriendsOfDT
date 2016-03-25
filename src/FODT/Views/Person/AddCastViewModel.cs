using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FODT.Models.IMDT;

namespace FODT.Views.Person
{
    public class AddCastViewModel
    {
        public AddCastViewModel(IEnumerable<FODT.Models.IMDT.Show> shows)
        {
            ShowOptions = shows
                .OrderBy(x => x, ShowComparer.ReverseChronologicalShowComparer)
                .Select(x => new KeyValuePair<string, string>(x.ShowId.ToString(), x.DisplayTitleWithYear))
                .ToList();
        }

        public string POSTUrl { get; set; }

        public List<KeyValuePair<string, string>> ShowOptions { get; set; }
    }
}