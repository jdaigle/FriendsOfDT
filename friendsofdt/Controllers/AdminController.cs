using System.Web.Mvc;
using FriendsOfDT.Models.Accounts;
using System.Collections.Generic;

namespace FriendsOfDT.Controllers {
    [Authorize]
    public partial class AdminController : AbstractController {
        
        public virtual RedirectToRouteResult Index() {
            return RedirectToAction(MVC.Admin.Dashboard());
        }

        public virtual ViewResult Dashboard() {
            return View();
        }

        public static bool CanSeeAdminLink(IEnumerable<WebAccountRole> roles) {
            return true;
        }
    }
}