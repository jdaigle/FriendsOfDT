using System;
using System.Linq;
using Raven.Client;

namespace FODT.Models
{
    public static class IDocumentSessionExtensions
    {
        public static string MakeId<T>(this IDocumentSession session, object id)
        {
            return session.Advanced.DocumentStore.Conventions.FindFullDocumentKeyFromNonStringIdentifier(id, typeof(T), false);
        }

        public static T GetId<T>(this IDocumentSession session, string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return default(T);
            }
            var actualType = typeof(T);
            if (actualType.IsGenericType && actualType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                actualType = actualType.GetGenericArguments()[0];
            }
            var converter =
                    session.Advanced.DocumentStore.Conventions.IdentityTypeConvertors.FirstOrDefault(x => x.CanConvertFrom(actualType));
            if (converter == null)
            {
                throw new ArgumentException("Could not convert identity to type " + typeof(T) +
                                            " because there is not matching type converter registered in the conventions' IdentityTypeConvertors");
            }

            return (T)converter.ConvertTo(session.Advanced.DocumentStore.Conventions.FindIdValuePartForValueTypeConversion(null, id));
        }
    }
}
