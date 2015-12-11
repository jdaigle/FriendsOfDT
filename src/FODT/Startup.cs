using Microsoft.Owin;
using Microsoft.Owin.Extensions;
using Owin;
using FODT;
using System.Collections.Generic;
using FODT.Security;

[assembly: OwinStartup(typeof(Startup))]
namespace FODT
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.Use<CookieAuthenticationMiddleware>(app);
            app.UseStageMarker(PipelineStage.Authenticate);
            //app.UseFacebookAuthentication(new FacebookAuthenticationOptions()
            //{
            //    AppId = "237050446406393",
            //    AppSecret = "7f47d8c38d291a0fc9e61769e3c61b71",
            //    Scope = new List<string>() { "public_profile", "email" },
            //});
        }
    }
}