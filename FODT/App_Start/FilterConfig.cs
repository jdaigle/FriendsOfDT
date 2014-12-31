using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using FODT.Controllers;
using FODT.Security;

namespace FODT
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            //filters.Add(new HandleErrorAttribute());
            filters.Add(new LoadPrincipalAttribute(LookupPrincipal));
            filters.Add(new LegacyUrlFilter());
        }

        public static IPrincipal LookupPrincipal(ControllerContext controllerContext, AuthenticationTokenIdentity identity, IPrincipal unknownPrincipal)
        {
            using (var session = FODT.Database.DatabaseBootstrapper.SessionFactory.OpenStatelessSession())
            {
                // TODO: query to 1. make sure the access token is valid in our database and 2. get profile/roles, etc
                var user = session.Get<FODT.Models.FODT.UserAccount>(identity.UserAccountId);
                if (user == null)
                {
                    return unknownPrincipal;
                }
                return new GenericPrincipal(identity, new string[0]);
            }

        }
    }
}