using System;
using Raven.Client.Listeners;
using Raven.Json.Linq;
using System.Web;

namespace FriendsOfDT {
    public class DocumentAuditListener : IDocumentStoreListener {
        public bool BeforeStore(string key, object entity, RavenJObject metadata) {
            if (entity == null)
                return false;
            if (!metadata.Value<DateTime?>("Created").HasValue) {
                metadata["Created"] = DateTime.UtcNow;
            }
            metadata["Last-Modified-Action"] = HttpContext.Current.Request.Path;
            if (HttpContext.Current.User != null &&
                HttpContext.Current.User.Identity != null &&
                !string.IsNullOrEmpty(HttpContext.Current.User.Identity.Name)) {
                metadata["Last-Modified-By"] = HttpContext.Current.User.Identity.Name;
            } else {
                metadata["Last-Modified-By"] = "system";
            }
            return false;
        }
        public void AfterStore(string key, object entity, RavenJObject metadata) {
        }
    }
}