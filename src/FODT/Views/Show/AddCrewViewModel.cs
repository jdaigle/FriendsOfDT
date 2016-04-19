using System.Collections.Generic;
using System.Linq;
using FODT.Models.IMDT;

namespace FODT.Views.Show
{
    public class AddCrewViewModel
    {
        public AddCrewViewModel(IEnumerable<FODT.Models.IMDT.Person> people, IEnumerable<string> crewPositions)
        {
            PersonOptions = people
                .OrderBy(x => x.SortableName)
                .Select(x => new KeyValuePair<string, string>(x.PersonId.ToString(), x.SortableName))
                .ToList();

            CrewPositionsJSONArray = string.Join(", ", crewPositions.OrderBy(x => x).Select(x => "\"" + x.Replace("\\", "\\\\").Replace("\"", "&quot;") + "\""));
        }

        public string POSTUrl { get; set; }

        public List<KeyValuePair<string, string>> PersonOptions { get; set; }

        public string CrewPositionsJSONArray { get; private set; }
    }
}