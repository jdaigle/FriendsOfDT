using FODT.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FODT.Views.Admin
{
    public class AdminTabsViewModel
    {
        public AdminTabsViewModel(UrlHelper url)
        {
            UserAdminListURL = url.GetURL<UserAdminController>(c => c.List());
            AwardsAdminURL = url.GetURL<AwardsAdminController>(c => c.Admin());
        }

        public string UserAdminListURL { get; set; }
        public string AwardsAdminURL { get; set; }
    }
}