using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FODT.Models.FODT;

namespace FODT.Security
{
    public interface IAuthenticationManager
    {
        void SignIn(string authenticationTokenId, string authenticationType);
        void SignOut();

        AuthenticationToken SignInToken { get; }
        bool IsSignOut { get; }
    }
}