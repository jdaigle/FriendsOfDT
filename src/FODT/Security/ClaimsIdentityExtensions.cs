using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;

namespace FODT.Security
{
    public static class ClaimsIdentityExtensions
    {
        public static int GetUserAccountId(this ClaimsIdentity identity)
        {
            return Convert.ToInt32(identity.FindFirst(c => c.Type == ClaimTypes.NameIdentifier).Value);
        }

        public static int GetUserAccountId(this ClaimsPrincipal principal)
        {
            return Convert.ToInt32(principal.FindFirst(c => c.Type == ClaimTypes.NameIdentifier).Value);
        }
    }
}