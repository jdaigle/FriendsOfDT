using System.Web.Mvc;
using FriendsOfDT.Models.Accounts;
using Raven.Client;

namespace FriendsOfDT.Controllers {
    public class AccountsController : Controller {
        private readonly IDocumentSession documentSession;

        public AccountsController(IDocumentSession documentSession) {
            this.documentSession = documentSession;
        }

        [HttpPost, AjaxOnly]
        public RenderJsonResult RegisterNewWebAccount(RegisterNewAccountParameters parameters) {
            var existingAccount = documentSession.Load<WebAccount>(parameters.EmailAddress);
            if (existingAccount != null) {
                return this.RenderJsonErrorCode(1, "An account already exists with this e-mail address.");
            }
            var newAccount = WebAccount.RegisterNewAccount(parameters);
            documentSession.Store(newAccount);
            // TODO: Publish event for e-mail notification
            return this.RenderJsonSuccessErrorCode();
        }

        [HttpPost, AjaxOnly]
        public RenderJsonResult VerifyWebAccount(string webAccountId) {
            var account = documentSession.Load<WebAccount>(webAccountId);
            if (account == null) {
                return this.RenderJsonErrorCode(1, "Missing Account");
            }
            account.Verify();
            return this.RenderJsonSuccessErrorCode();
        }
    }
}
