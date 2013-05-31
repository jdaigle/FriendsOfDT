using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AttributeRouting;
using AttributeRouting.Web.Mvc;

namespace FODT.Controllers
{
    [RoutePrefix("")]
    public partial class HomeController : BaseController
    {
        [GET("")]
        public virtual ActionResult Welcome()
        {
            return View();
        }

        [GET("Index")]
        public virtual ActionResult Index()
        {
            return View();
        }
    }
}
