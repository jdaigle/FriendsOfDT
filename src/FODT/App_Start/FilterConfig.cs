using System.Web.Mvc;
using FODT.Security;

namespace FODT
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            //filters.Add(new HandleErrorAttribute());
            filters.Add(new CookieAuthenticationFilter());
            filters.Add(new LegacyUrlFilter());
        }
    }
}