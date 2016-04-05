using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FODT.Controllers;
using FODT.Models.IMDT;

namespace FODT.Views.AwardsAdmin
{
    public class AwardTypeViewModel
    {
        public AwardTypeViewModel() { }

        public AwardTypeViewModel(AwardType awardType, UrlHelper url)
        {
            Name = awardType.Name;
            EditURL = url.GetURL<AwardsAdminController>(c => c.EditAwardType(awardType.AwardTypeId));
        }

        public string EditURL { get; set; }
        public string Name { get; set; }
    }
}