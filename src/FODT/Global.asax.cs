using StackExchange.Profiling;

namespace FODT
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            Startup.ApplicationStart();
        }

        protected void Application_BeginRequest()
        {
#if DEBUG
            MiniProfiler.Start();
#endif
        }

        protected void Application_EndRequest()
        {
#if DEBUG
            MiniProfiler.Stop();
#endif
        }
    }
}