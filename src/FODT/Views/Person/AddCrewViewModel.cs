using System.Collections.Generic;
using System.Linq;
using FODT.Models.IMDT;

namespace FODT.Views.Person
{
    public class AddCrewViewModel
    {
        public AddCrewViewModel(IEnumerable<FODT.Models.IMDT.Show> shows, IEnumerable<string> crewPositions)
        {
            ShowOptions = shows
                .OrderBy(x => x, ShowComparer.ReverseChronologicalShowComparer)
                .Select(x => new KeyValuePair<string, string>(x.ShowId.ToString(), x.DisplayTitleWithYear))
                .ToList();

            CrewPositionsJSONArray = string.Join(", ", crewPositions.OrderBy(x => x).Select(x => "\"" + x.Replace("\\", "\\\\").Replace("\"", "&quot;") + "\""));
        }

        public string POSTUrl { get; set; }

        public List<KeyValuePair<string, string>> ShowOptions { get; set; }

        public string CrewPositionsJSONArray { get; private set; }
    }
}