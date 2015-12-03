using System.Security.Principal;

namespace FODT.Security
{
    public interface IAuthenticationTokenContext
    {
        AuthenticationToken IssueAuthenticationToken(int userAccountId, string accessToken, string name, string authenticationType, int expiresInSeconds);
        void RevokeAuthenticationToken();
        AuthenticationToken GetCurrentAuthenticationToken();
        bool IsAuthenticated();
    }
}
