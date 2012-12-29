using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FODT.Models.IMDT.Indexes
{
    public class CrewProjection
    {
        public string __document_id { get; set; } // show id
        public string PersonId { get; set; }
        public string PersonName { get; set; }
        public string ShowName { get; set; }
        public Quarter ShowQuarter { get; set; }
        public short ShowYear { get; set; }
        public int CrewPositionId { get; set; }
    }
}