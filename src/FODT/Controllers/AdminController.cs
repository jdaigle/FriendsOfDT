using System.Web.Mvc;
using FODT.Views.Admin;

namespace FODT.Controllers
{
    public class AdminController : BaseController
    {
        [Route("admin/components/tabs")]
        [ChildActionOnly]
        public ActionResult GetAdminTabs()
        {
            return PartialView(new AdminTabsViewModel(Url));
        }
    }
}