using System.Linq;
using Raven.Client.Listeners;
using Raven.Json.Linq;

namespace FriendsOfDT {
    public class DocumentVersionStoreListener : IDocumentStoreListener {
        public bool BeforeStore(string key, object entity, RavenJObject metadata) {
            if (entity == null)
                return false;
            var metadataAttribute = entity.GetType().GetCustomAttributes(typeof(EntityMetadataAttribute), false).FirstOrDefault() as EntityMetadataAttribute;
            if (metadataAttribute == null)
                return false;
            if (metadata.Value<int>("Entity-Class-Version") < metadataAttribute.Version)
                metadata["Entity-Class-Version"] = metadataAttribute.Version;
            return false;
        }
        public void AfterStore(string key, object entity, RavenJObject metadata) {
        }
    }
}