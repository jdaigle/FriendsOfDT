using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FODT.Views.Extensions
{
    public static class DebugExtensions
    {
        public static bool IsDebug(this WebViewPage view)
        {
#if DEBUG
            return true;
#else
            return false;
#endif
        }
    }
}