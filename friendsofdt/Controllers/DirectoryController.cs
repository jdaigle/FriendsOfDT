using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FriendsOfDT.Controllers
{
    public partial class DirectoryController : AbstractController
    {
        public virtual ViewResult Index()
        {
            return View();
        }

        public virtual ViewResult ViewProfile() {
            return View();
        }
    }
}
