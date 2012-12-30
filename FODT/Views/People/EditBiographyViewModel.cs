using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FODT.Views.People
{
    public class EditBiographyViewModel
    {
        public int PersonId { get; set; }
        public string Name { get; set; }
        public string Biography { get; set; }
    }
}