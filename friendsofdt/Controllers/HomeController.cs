using System.Web.Mvc;

namespace friendsofdt.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return Content("Coming Soon");
        }
    }
}