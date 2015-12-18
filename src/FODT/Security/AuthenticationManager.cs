using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using FODT.Models.FODT;

namespace FODT.Security
{
    public class AuthenticationManager : IAuthenticationManager
    {
        public void SignIn(string authenticationTokenId, string authenticationType)
        {
            IsSignOut = false;
            var id = new ClaimsIdentity(authenticationType, ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
            id.AddClaim(new Claim(ClaimTypes.NameIdentifier, authenticationTokenId, ClaimValueTypes.String));
            SignInToken = new AuthenticationToken(id, new AuthenticationProperties()
            {
                IsPersistent = true,
                IssuedUtc = DateTime.UtcNow,
            });
        }

        public void SignOut()
        {
            SignInToken = null;
            IsSignOut = true;
        }

        public AuthenticationToken SignInToken { get; private set; }
        public bool IsSignOut { get; private set; }
    }
}