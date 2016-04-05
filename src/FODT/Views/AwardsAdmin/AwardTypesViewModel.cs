using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FODT.Controllers;
using FODT.Models.IMDT;

namespace FODT.Views.AwardsAdmin
{
    public class AwardTypesViewModel
    {
        public AwardTypesViewModel(List<AwardType> awardTypes, UrlHelper url)
        {
            this.AddUrl = url.GetURL<AwardsAdminController>(c => c.POSTAddEditAwardType(null));
            this.AwardTypes = awardTypes
                .Select(x => new AwardTypeViewModel
                {
                    Name = x.Name,
                    EditURL = url.GetURL<AwardsAdminController>(c => c.EditAwardType(x.AwardTypeId)),
                })
                .OrderBy(x => x.Name)
                .ToList();
        }

        public string AddUrl { get; }
        public List<AwardTypeViewModel> AwardTypes { get; }

        public class AwardTypeViewModel
        {
            public string Name { get; set; }
            public string EditURL { get; set; }
        }
    }
}