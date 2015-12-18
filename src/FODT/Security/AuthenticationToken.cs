using System.Security.Claims;
using Microsoft.Owin.Security;

namespace FODT.Security
{
    public sealed class AuthenticationToken
    {
        public AuthenticationToken(ClaimsIdentity identity, AuthenticationProperties properties)
        {
            Identity = identity;
            Properties = properties ?? new AuthenticationProperties();
        }

        public ClaimsIdentity Identity { get; private set; }
        public AuthenticationProperties Properties { get; private set; }
    }
}
