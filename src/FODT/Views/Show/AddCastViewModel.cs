using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FODT.Models.IMDT;

namespace FODT.Views.Show
{
    public class AddCastViewModel
    {
        public AddCastViewModel(IEnumerable<FODT.Models.IMDT.Person> people)
        {
            PersonOptions = people
                .OrderBy(x => x.SortableName)
                .Select(x => new KeyValuePair<string, string>(x.PersonId.ToString(), x.SortableName))
                .ToList();
        }

        public string POSTUrl { get; set; }

        public List<KeyValuePair<string, string>> PersonOptions { get; set; }
    }
}