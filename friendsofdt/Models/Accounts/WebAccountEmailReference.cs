namespace FriendsOfDT.Models.Accounts {
    [EntityMetadata(Version = 1)]
    public class WebAccountEmailReference {
        public static string GetId(string emailAddress) {
            return "webAccounts/emails/" + emailAddress;
        }

        protected WebAccountEmailReference() { }

        public WebAccountEmailReference(string emailAddress, string webAccountId) {
            this.Id = GetId(emailAddress);
            this.WebAccountId = webAccountId;
        }

        public string Id { get; protected set; }
        public string WebAccountId { get; protected set; }
    }
}