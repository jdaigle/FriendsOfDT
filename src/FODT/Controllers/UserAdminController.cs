using FODT.Models.FODT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NHibernate.Linq;
using FODT.Views.UserAdmin;
using System.Globalization;
using Microsoft.Web.Mvc;

namespace FODT.Controllers
{
    [RoutePrefix("Admin")]
    public class UserAdminController : BaseController
    {
        [Route("Users")]
        public ActionResult Index()
        {
            return this.RedirectToAction(c => c.List());
        }

        [Route("Users/List")]
        public ActionResult List()
        {
            var users = DatabaseSession.Query<UserAccount>().ToList();

            var listViewModel = new ListViewModel();
            listViewModel.Users = users.Select(x => new UserViewModel(x, this.Url)).ToList();
            return View(listViewModel);
        }

        [HttpGet]
        [AjaxOnly]
        [Route("User/{userId:int}/edit")]
        public ActionResult EditUser(int userId)
        {
            var user = DatabaseSession.Get<UserAccount>(userId);
            var userViewModel = new UserViewModel(user, this.Url);
            return PartialView(userViewModel);
        }

        [HttpPost]
        [Route("User/{userId:int}/edit")]
        public ActionResult POSTEditUser(int userId, UserViewModel postModel)
        {
            if (Request.IsAjaxRequest())
            {
                return Json("OK");
            }
            return this.RedirectToAction(c => c.List());
        }
    }
}