using System;
using Raven.Json.Linq;

namespace FriendsOfDT {
    public class EntityMetadata {

        public static EntityMetadata FromRaven(RavenJObject metadata) {
            return new EntityMetadata() {
                ETag = metadata.Value<Guid?>("@etag"),
                Created = metadata.Value<DateTime?>("Created"),
                LastModified = metadata.Value<DateTime?>("Last-Modified"),
                LastModifiedBy = metadata.Value<string>("Last-Modified-By"),
                LastModifiedAction = metadata.Value<string>("Last-Modified-Action"),
                EntityClassVersion = metadata.Value<int?>("Entity-Class-Version"),
                RavenEntityName = metadata.Value<string>("Raven-Entity-Name"),
                RavenClrType = metadata.Value<string>("Raven-Clr-Type"),
            };
        }

        public string RavenEntityName { get; set; }
        public string RavenClrType { get; set; }
        public int? EntityClassVersion { get; set; }
        public Guid? ETag { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? LastModified { get; set; }
        public string LastModifiedBy { get; set; }
        public string LastModifiedAction { get; set; }
    }
}