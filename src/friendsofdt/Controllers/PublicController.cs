using System.Web.Mvc;

namespace FriendsOfDT.Controllers
{
    public partial class PublicController : AbstractController
    {
        public virtual ActionResult Index()
        {
            return View();
        }
    }
}