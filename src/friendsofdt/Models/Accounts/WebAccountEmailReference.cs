namespace FriendsOfDT.Models.Accounts {
    [EntityMetadata(Version = 1)]
    public class WebAccountEmailReference {
        protected WebAccountEmailReference() { }

        public WebAccountEmailReference(string id, string webAccountId) {
            this.Id = id;
            this.WebAccountId = webAccountId;
        }

        public string Id { get; protected set; }
        public string WebAccountId { get; protected set; }
    }
}