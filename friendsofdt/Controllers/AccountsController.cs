using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using FriendsOfDT.Models.Accounts;
using Raven.Client.Linq;
using RiaLibrary.Web;

namespace FriendsOfDT.Controllers {
    public partial class AccountsController : AbstractController {
        [HttpGet]
        public virtual RedirectToRouteResult SignOut() {
            FormsAuthentication.SignOut();
            return RedirectToAction(MVC.Public.Index());
        }

        [AjaxOnly, HttpPost, ValidateInput(false)]
        public virtual RenderJsonResult Login(string emailAddress, string password, bool persist, string returnUrl) {
            var accountEmailReference = DocumentSession.Query<WebAccountEmailReference>()
                .Where(x => x.Id == WebAccountEmailReference.GetId(emailAddress))
                .SingleOrDefault();
            if (accountEmailReference == null) {
                return this.RenderJsonErrorCode(1, "Bad Username or Password");
            }
            var webAccount = DocumentSession.Load<WebAccount>(accountEmailReference.WebAccountId);
            if (webAccount == null || !webAccount.PasswordMatches(password)) {
                return this.RenderJsonErrorCode(1, "Bad Username or Password");
            }
            if (!webAccount.CanLogin()) {
                return this.RenderJsonErrorCode(2, "Account is locked");
            }
            FormsAuthentication.SetAuthCookie(webAccount.Id, persist);
            SetRoles(webAccount.Roles);
            return new RenderJsonResult() { Data = new { redirect = Url.IsLocalUrl(returnUrl) ? returnUrl : Url.Action(MVC.Public.Index()) } };
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
            if (string.IsNullOrWhiteSpace(requestedPassword) || requestedPassword != confirmedPassword) {
                DocumentSession.Advanced.Clear();
                return this.RenderJsonErrorCode(2, "A password is required, both passwords must match");
            }
            var newAccount = WebAccount.RegisterNewAccount(parameters);
            newAccount.ChangePassword(requestedPassword);
            var newAccountEmailReference = new WebAccountEmailReference(newAccount.EmailAddress, newAccount.Id);
            DocumentSession.Store(newAccount);
            DocumentSession.Store(newAccountEmailReference);
            // TODO: Publish event for e-mail notification
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

        [Authorize, AuthorizeRole()]
        [HttpGet, Url("Admin/WebAccounts/List")]
        public virtual ViewResult AdminList() {
            return View();
        }

        [AjaxOnly]
        [Authorize, AuthorizeRole()]
        public virtual RenderJsonResult ListAccounts(int? page, int? itemsPerPage) {
            page = page ?? 1;
            itemsPerPage = itemsPerPage ?? 20;
            RavenQueryStatistics stats = null;
            var results = DocumentSession.Query<WebAccount>()
                .Statistics(out stats)
                //.OrderBy(x => x.LastName)
                .OrderBy(x => x.EmailAddress)
                .Page(page.Value, itemsPerPage.Value)
                .ToList()
                .Select(x => new { id = x.Id, emailAddress = x.EmailAddress, registrationStatus = x.RegistrationStatus.ToString() }).ToList();
            return new RenderJsonResult() { Data = new { items = results, count = stats.TotalResults } };
        }

        [Authorize, AuthorizeRole()]
        [HttpGet, Url("Admin/WebAccounts/{id}/Manage")]
        public virtual ViewResult Manage(string id) {
            var account = DocumentSession.Query<WebAccount>("webAccounts/" + id);
            return View();
        }
    }
}
