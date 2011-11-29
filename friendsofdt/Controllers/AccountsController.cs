using System.Web.Mvc;
using FriendsOfDT.Models.Accounts;
using Raven.Client;
using System.Threading;

namespace FriendsOfDT.Controllers {
    public partial class AccountsController : AbstractController {
        [HttpGet]
        public virtual ViewResult Login() {
            return View();
        }

        [HttpPost, ValidateInput(false)]
        public virtual RedirectToRouteResult Login(string emailAddress, string password) {
            return RedirectToAction(MVC.Public.Index());
        }

        [HttpGet]
        public virtual ViewResult SignUp() {
            return View();
        }

        [HttpPost, AjaxOnly, ValidateInput(false)]
        public virtual RenderJsonResult RegisterNewWebAccount(RegisterNewAccountParameters parameters, string requestedPassword, string confirmedPassword) {
            var existingAccount = DocumentSession.Load<WebAccount>(parameters.EmailAddress);
            if (existingAccount != null) {
                return this.RenderJsonErrorCode(1, "An account already exists with this e-mail address.");
            }
            var newAccount = WebAccount.RegisterNewAccount(parameters);
            DocumentSession.Store(newAccount);
            // TODO: Publish event for e-mail notification
            if (string.IsNullOrWhiteSpace(requestedPassword) || requestedPassword != confirmedPassword) {
                return this.RenderJsonErrorCode(2, "A password is required, both passwords must match");
            }
            return this.RenderJsonSuccessErrorCode();
        }

        [HttpPost, AjaxOnly]
        public virtual RenderJsonResult VerifyWebAccount(string webAccountId) {
            var account = DocumentSession.Load<WebAccount>(webAccountId);
            if (account == null) {
                return this.RenderJsonErrorCode(1, "Missing Account");
            }
            account.Verify();
            return this.RenderJsonSuccessErrorCode();
        }
    }
}
