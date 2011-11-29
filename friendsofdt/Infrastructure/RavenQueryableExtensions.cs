using System.Linq;

namespace FriendsOfDT {
    public static class RavenQueryableExtensions {
        public static IQueryable<T> Page<T>(this IQueryable<T> query, int page, int itemsPerPage) {
            return query.Take(itemsPerPage).Skip((page + 1 * itemsPerPage) - itemsPerPage);
        }
    }
}