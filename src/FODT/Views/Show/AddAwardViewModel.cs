using System;
using System.Collections.Generic;
using System.Linq;
using FODT.Models.IMDT;

namespace FODT.Views.Show
{
    public class AddAwardViewModel
    {
        public AddAwardViewModel(IEnumerable<AwardType> awardTypes, IEnumerable<FODT.Models.IMDT.Person> people, short showYear)
        {
            ShowYear = showYear.ToString();
            AwardTypeOptions = awardTypes
                .OrderBy(x => x.Name)
                .Select(x => new KeyValuePair<string, string>(x.AwardTypeId.ToString(), x.Name))
                .ToList();
            PeopleOptions = people
                .OrderBy(x => x.SortableName)
                .Select(x => new KeyValuePair<string, string>(x.PersonId.ToString(), x.SortableName))
                .ToList();
        }

        public string POSTUrl { get; set; }

        public List<KeyValuePair<string,string>> PeopleOptions { get; set; }
        public List<KeyValuePair<string,string>> AwardTypeOptions { get; set; }

        public string ShowYear { get; set; }
    }
}