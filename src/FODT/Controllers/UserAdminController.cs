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
using System.Security.Claims;
using FODT.Security;

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
            listViewModel.IsUserAdmin = this.User.IsInRole(RoleNames.Admin);
            listViewModel.Users = users.Select(x => new UserAccountViewModel(x, this.Url)).ToList();
            return View(listViewModel);
        }

        [HttpGet]
        [AjaxOnly]
        [Route("User/{userId:int}/edit")]
        public ActionResult EditUser(int userId)
        {
            var user = DatabaseSession.Get<UserAccount>(userId);
            var userViewModel = new UserAccountViewModel(user, this.Url);
            return PartialView(userViewModel);
        }

        [HttpPost]
        [Route("User/{userId:int}/edit")]
        public ActionResult POSTEditUser(int userId, UserAccountViewModel postModel)
        {
            var user = DatabaseSession.Get<UserAccount>(userId);
            if (user == null)
            {
                return new HttpNotFoundResult();
            }

            if (this.User.IsInRole(RoleNames.Admin) && ((ClaimsPrincipal)this.User).GetUserAccountId() == userId)
            {
                // do not allow admin to remove self from admin role
                postModel.IsAdmin = true;
            }

            user.IsContributor = postModel.IsContributor;
            user.IsArchivist = postModel.IsArchivist;
            user.IsAdmin = postModel.IsAdmin;

            if (Request.IsAjaxRequest())
            {
                return Json("OK");
            }

            return this.RedirectToAction(c => c.List());
        }
    }
}