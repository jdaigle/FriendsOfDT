using System;
using System.Collections.Generic;
using System.Web;

namespace FODT
{
    public static class ObjectDisposal
    {
        public static void DisposeAtEndRequest(IDisposable instance)
        {
            GetObjectsToDispose().Add(instance);
        }

        public static void DisposeAll()
        {
            foreach (var item in GetObjectsToDispose())
            {
                item.Dispose();
            }
        }

        private static IList<IDisposable> GetObjectsToDispose()
        {
            if (!HttpContext.Current.Items.Contains("ObjectsToDispose_AtEndRequest"))
            {
                var objectsToDispose = new List<IDisposable>();
                HttpContext.Current.Items.Add("ObjectsToDispose_AtEndRequest", objectsToDispose);
            }
            return HttpContext.Current.Items["ObjectsToDispose_AtEndRequest"] as IList<IDisposable>;
        }
    }
}