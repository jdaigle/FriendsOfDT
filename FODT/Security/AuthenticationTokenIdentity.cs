using System.Security.Principal;

namespace FODT.Security
{
    public class AuthenticationTokenIdentity : GenericIdentity
    {
        public AuthenticationTokenIdentity(AuthenticationToken authenticationToken)
            : base(authenticationToken.Name, authenticationToken.AuthenticationType)
        {
            this.AuthenticationToken = authenticationToken;
        }

        public int UserAccountId { get { return this.AuthenticationToken.UserAccountId; } }
        public string AccessToken { get { return this.AuthenticationToken.AccessToken; } }
        public AuthenticationToken AuthenticationToken { get; private set; }
    }
}