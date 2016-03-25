using System;
using System.Collections.Generic;
using System.Linq;
using FODT.Models.IMDT;

namespace FODT.Views.Person
{
    public class AddAwardViewModel
    {
        public AddAwardViewModel(IEnumerable<AwardType> awardTypes, IEnumerable<FODT.Models.IMDT.Show> shows)
        {
            CurrentYear = DateTime.Now.Year.ToString();
            AwardTypeOptions = awardTypes
                .OrderBy(x => x.Name)
                .Select(x => new KeyValuePair<string, string>(x.AwardTypeId.ToString(), x.Name))
                .ToList();
            ShowOptions = shows
                .OrderBy(x => x, ShowComparer.ReverseChronologicalShowComparer)
                .Select(x => new KeyValuePair<string, string>(x.ShowId.ToString(), x.DisplayTitleWithYear))
                .ToList();
        }

        public string POSTUrl { get; set; }

        public List<KeyValuePair<string,string>> ShowOptions { get; set; }
        public List<KeyValuePair<string,string>> AwardTypeOptions { get; set; }

        public string CurrentYear { get; set; }
    }
}