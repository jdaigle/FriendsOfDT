using System;
using System.Security.Claims;
using System.Security.Principal;

namespace FODT.Security
{
    public class GuestPrincipal : GenericPrincipal
    {
        public static GuestPrincipal Default = new GuestPrincipal();

        private class GuestIdentity : ClaimsIdentity
        {
            public GuestIdentity()
                : base("Guest")
            {

            }

            public override bool IsAuthenticated
            {
                get
                {
                    return false;
                }
            }
        }

        public GuestPrincipal() : base(new GuestIdentity(), Array.Empty<string>()) { }
    }
}
