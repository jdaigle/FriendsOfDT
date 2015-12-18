using System.Web;

namespace FODT
{
    public static class HttpContextExtensions
    {
        public static void Set<T>(this HttpContextBase httpContext, T value)
        {
            httpContext.Items[(typeof(T).FullName)] = value;
        }

        public static T Get<T>(this HttpContextBase httpContext)
        {
            return (T)httpContext.Items[(typeof(T).FullName)];
        }
    }
}