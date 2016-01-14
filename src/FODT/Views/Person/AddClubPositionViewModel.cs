using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FODT.Views.Person
{
    public class AddClubPositionViewModel
    {
        public AddClubPositionViewModel(IEnumerable<string> positions)
        {
            CurrentYear = DateTime.Now.Year.ToString();
            Positions = string.Join(", ", positions.OrderBy(x => x).Select(x => "\"" + x.Replace("\"", "&quot;") + "\""));
        }

        public string POSTUrl { get; set; }
        public string Positions { get; set; }
        public string CurrentYear { get; set; }
    }
}