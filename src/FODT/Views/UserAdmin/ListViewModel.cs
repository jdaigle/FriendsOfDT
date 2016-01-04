using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FODT.Views.UserAdmin
{
    public class ListViewModel
    {
        public bool IsUserAdmin { get; set; }
        public List<UserAccountViewModel> Users { get; set; }
    }
}