using System.Linq;

namespace FriendsOfDT {
    public static class RavenQueryableExtensions {
        public static IQueryable<T> Page<T>(this IQueryable<T> query, int page, int itemsPerPage) {
            if (page < 1) page = 1;
            return query.Take(itemsPerPage).Skip((page * itemsPerPage) - itemsPerPage);
        }
    }
}