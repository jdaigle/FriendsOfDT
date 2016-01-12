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
            MiniProfiler.Start();
        }

        protected void Application_EndRequest()
        {
            MiniProfiler.Stop();
        }
    }
}