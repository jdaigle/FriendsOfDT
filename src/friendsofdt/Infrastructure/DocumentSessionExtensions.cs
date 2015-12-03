using Raven.Client;

namespace FriendsOfDT {
    public static class DocumentSessionExtensions {
        public static long GetEntityIdValue(this IDocumentSession documentSession, object entity) {
            var entityId = (string)documentSession.Advanced.DocumentStore.Conventions.GetIdentityProperty(entity.GetType()).GetValue(entity, null);
            return GetEntityIdValue(documentSession, entity, entityId);
        }

        public static long GetEntityIdValue(this IDocumentSession documentSession, object entity, string entityId) {
            return long.Parse(documentSession.Advanced.DocumentStore.Conventions.FindIdValuePartForValueTypeConversion(entity, entityId));
        }

        public static string GetEntityIdFromValue<TEntity>(this IDocumentSession documentSession, object id) {
            return documentSession.Advanced.DocumentStore.Conventions.FindFullDocumentKeyFromNonStringIdentifier(id, typeof(TEntity), false);
        }

        public static string GetEntityIdFromValue(this IDocumentSession documentSession, object id, object entity) {
            return documentSession.Advanced.DocumentStore.Conventions.FindFullDocumentKeyFromNonStringIdentifier(id, entity.GetType(), false);
        }

        public static string GetEntityTagName<TEntity>(this IDocumentSession documentSession) {
            return documentSession.Advanced.DocumentStore.Conventions.GetTypeTagName(typeof(TEntity));
        }

        public static string GetEntityTagName(this IDocumentSession documentSession, object entity) {
            return documentSession.Advanced.DocumentStore.Conventions.GetTypeTagName(entity.GetType());
        }
    }
}