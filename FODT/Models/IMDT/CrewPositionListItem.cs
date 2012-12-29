using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FODT.Models.IMDT
{
    [Serializable]
    public class CrewPositionListItem
    {
        public int Key { get; set; }
        public string Name { get; set; }
        public int Priority { get; set; }
        public string DefinitionURL { get; set; }
    }
}
