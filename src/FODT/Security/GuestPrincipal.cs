using System.Security.Principal;

namespace FODT.Security
{
    public class GuestPrincipal : GenericPrincipal
    {
        public class GuestIdentity : GenericIdentity
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

        public GuestPrincipal(IIdentity identity, string[] roles)
            : base(identity, roles)
        {
        }

        public GuestPrincipal(IIdentity identity)
            : this(identity, new string[0])
        {
        }
    }
}
