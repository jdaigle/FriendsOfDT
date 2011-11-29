using System.Web.Mvc;
using System.Linq;
using FriendsOfDT.Models.Accounts;
using System;
using System.Web.Security;

namespace FriendsOfDT.Controllers {
    public partial class AccountsController : AbstractController {
        [HttpGet]
        public virtual ViewResult Login() {
            return View();
        }

        [HttpGet]
        public virtual RedirectToRouteResult SignOut() {
            FormsAuthentication.SignOut();
            return RedirectToAction(MVC.Public.Index());
        }

        [HttpPost, ValidateInput(false)]
        public virtual RedirectToRouteResult Login(string emailAddress, string password, bool persist) {
            var accountEmailReference = DocumentSession.Query<WebAccountEmailReference>()
                .Where(x => x.Id == WebAccountEmailReference.GetId(emailAddress))
                .SingleOrDefault();
            if (accountEmailReference == null) {
                throw new Exception("Bad User");
            }
            var accountPassword = DocumentSession.Query<WebAccountPassword>()
                .Where(x => x.WebAccountId == accountEmailReference.WebAccountId)
                .SingleOrDefault();
            if (accountPassword == null || !accountPassword.PasswordMatches(password)) {
                throw new Exception("Bad Password");
            }
            var webAccount = DocumentSession.Load<WebAccount>(accountEmailReference.WebAccountId);
            if (!webAccount.CanLogin()) {
                throw new Exception("Invalid Login");
            }
            FormsAuthentication.SetAuthCookie(webAccount.EmailAddress, persist);
            return RedirectToAction(MVC.Public.Index());
        }

        [HttpGet]
        public virtual ViewResult SignUp() {
            return View();
        }

        [HttpPost, AjaxOnly, ValidateInput(false)]
        public virtual RenderJsonResult RegisterNewWebAccount(RegisterNewAccountParameters parameters, string requestedPassword, string confirmedPassword) {
            var existingAccountEmailReference = DocumentSession.Load<WebAccountEmailReference>(WebAccountEmailReference.GetId(parameters.EmailAddress));
            if (existingAccountEmailReference != null) {
                DocumentSession.Advanced.Clear();
                return this.RenderJsonErrorCode(1, "An account already exists with this e-mail address.");
            }
            var newAccount = WebAccount.RegisterNewAccount(parameters);
            var newAccountEmailReference = new WebAccountEmailReference(newAccount.EmailAddress, newAccount.Id);
            DocumentSession.Store(newAccount);
            DocumentSession.Store(newAccountEmailReference);
            // TODO: Publish event for e-mail notification
            if (string.IsNullOrWhiteSpace(requestedPassword) || requestedPassword != confirmedPassword) {
                DocumentSession.Advanced.Clear();
                return this.RenderJsonErrorCode(2, "A password is required, both passwords must match");
            }
            var password = new WebAccountPassword(newAccount.Id);
            password.ChangePassword(requestedPassword);
            DocumentSession.Store(password);
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
