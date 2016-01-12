using System;
using System.Linq;
using System.Web;

namespace FODT
{
    public static class HttpRequestExtensions
    {
        public static bool Accepts(this HttpRequestBase request, params string[] types)
        {
            return request.AcceptTypes.Intersect(types, StringComparer.InvariantCultureIgnoreCase).Any();
        }
    }
}