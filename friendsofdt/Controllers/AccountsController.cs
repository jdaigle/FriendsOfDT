using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using FriendsOfDT.Models.Accounts;
using Raven.Client.Linq;
using RiaLibrary.Web;

namespace FriendsOfDT.Controllers {
    public partial class AccountsController : AbstractController {

        [HttpGet]
        public virtual ViewResult Required() {
            return View();
        }

        [HttpGet]
        public virtual RedirectToRouteResult SignOut() {
            FormsAuthentication.SignOut();
            return RedirectToAction(MVC.Public.Index());
        }

        [AjaxOnly, HttpPost, ValidateInput(false)]
        public virtual RenderJsonResult Login(string emailAddress, string password, bool persist, string returnUrl) {
            var webAccountEmailReferenceId = DocumentSession.GetEntityIdFromValue<WebAccountEmailReference>(emailAddress);
            var accountEmailReference = DocumentSession.Load<WebAccountEmailReference>(webAccountEmailReferenceId);
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
            webAccount.IncrementLogin();
            FormsAuthentication.SetAuthCookie(webAccount.Id, persist);
            SetRoles(webAccount.Roles);
            return new RenderJsonResult() { Data = new { redirect = Url.IsLocalUrl(returnUrl) ? returnUrl : Url.Action(MVC.Public.Index()) } };
        }

        [HttpGet]
        public virtual ViewResult SignUp() {
            return View();
        }

        [HttpPost, AjaxOnly, ValidateInput(false)]
        public virtual RenderJsonResult RegisterNewWebAccount(RegisterNewAccountParameters parameters) {
            var existingWebAccountEmailReferenceId = DocumentSession.GetEntityIdFromValue<WebAccountEmailReference>(parameters.EmailAddress);
            var existingWebAccountEmailReference = DocumentSession.Load<WebAccountEmailReference>(existingWebAccountEmailReferenceId);
            if (existingWebAccountEmailReference != null) {
                DocumentSession.Advanced.Clear();
                return this.RenderJsonErrorCode(1, "An account already exists with this e-mail address.");
            }
            if (string.IsNullOrWhiteSpace(parameters.RequestedPassword)) {
                DocumentSession.Advanced.Clear();
                return this.RenderJsonErrorCode(2, "A password is required, both passwords must match");
            }
            var newAccount = WebAccount.RegisterNewAccount(parameters);
            newAccount.ChangePassword(parameters.RequestedPassword);
            DocumentSession.Store(newAccount);

            var newAccountEmailReference = new WebAccountEmailReference(existingWebAccountEmailReferenceId, newAccount.Id);
            DocumentSession.Store(newAccountEmailReference);

            // TODO: Publish event for e-mail notification
            return this.RenderJsonSuccessErrorCode();
        }

        [HttpPost, AjaxOnly]
        public virtual RenderJsonResult VerifyWebAccount(long webAccountId) {
            var account = DocumentSession.Load<WebAccount>(webAccountId);
            if (account == null) {
                return this.RenderJsonErrorCode(1, "Missing Account");
            }
            account.Verify();
            return this.RenderJsonSuccessErrorCode();
        }

        [HttpPost, AjaxOnly]
        public virtual RenderJsonResult DisableWebAccount(long webAccountId) {
            var account = DocumentSession.Load<WebAccount>(webAccountId);
            if (account == null) {
                return this.RenderJsonErrorCode(1, "Missing Account");
            }
            account.Disable();
            return this.RenderJsonSuccessErrorCode();
        }

        [HttpPost, AjaxOnly]
        public virtual RenderJsonResult EnableWebAccount(long webAccountId) {
            var account = DocumentSession.Load<WebAccount>(webAccountId);
            if (account == null) {
                return this.RenderJsonErrorCode(1, "Missing Account");
            }
            account.Enable();
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
                .OrderBy(x => x.LastName).OrderBy(x => x.FirstName)
                .Page(page.Value, itemsPerPage.Value)
                .ToList()
                .Select(x => new { id = x.Id, lastName = x.LastName, firstName = x.FirstName, emailAddress = x.EmailAddress, registrationStatus = x.RegistrationStatus.ToString() }).ToList();
            return new RenderJsonResult() { Data = new { items = results, count = stats.TotalResults } };
        }

        [Authorize, AuthorizeRole()]
        [HttpGet, Url("Admin/WebAccounts/{id}/Manage")]
        public virtual ActionResult Manage(long id) {
            var account = DocumentSession.Load<WebAccount>(id);
            if (account == null) {
                return Content("Account Not Found");
            }
            var metadata = EntityMetadata.FromRaven(DocumentSession.Advanced.GetMetadataFor(account));
            ViewBag.Metadata = metadata;
            if (!string.IsNullOrWhiteSpace(metadata.LastModifiedBy)) {
                var modifiedBy = DocumentSession.Load<WebAccount>(metadata.LastModifiedBy);
                if (modifiedBy != null) {
                    ViewBag.ModifiedBy = modifiedBy;
                    ViewBag.ModifiedById = DocumentSession.GetEntityIdValue(modifiedBy);
                }
            }
            ViewBag.Id = id;
            return View(account);
        }

        [Authorize, AuthorizeRole()]
        [HttpGet, Url("Admin/WebAccounts/{id}/Link")]
        public virtual ActionResult Link(long id) {
            var account = DocumentSession.Load<WebAccount>(id);
            if (account == null) {
                return Content("Account Not Found");
            }
            ViewBag.Id = id;
            return View(account);
        }
    }
}
