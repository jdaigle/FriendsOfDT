using System;
using System.Collections.Concurrent;
using System.Reflection;
using System.Runtime.Serialization;
using System.Web.Mvc;
using System.Web.Routing;

namespace FODT
{
    /*
    * This class is derived from the awesome concept and sample code in this blog post: http://maxtoroq.github.io/2013/04/delegate-based-strongly-typed-url.html
    */
    public static class DelegateUrlHelperExtensions
    {
        private static readonly ConcurrentDictionary<Type, object> uninitializedControllerCache = new ConcurrentDictionary<Type, object>();
        private static readonly ConcurrentDictionary<MethodInfo, string> actionNames = new ConcurrentDictionary<MethodInfo, string>();
        private static readonly ConcurrentDictionary<MethodInfo, string> controllerNames = new ConcurrentDictionary<MethodInfo, string>();

        public static T InstanceOf<T>() where T : ControllerBase
        {
            return (T)uninitializedControllerCache.GetOrAdd(typeof(T), type =>
            {
                return FormatterServices.GetUninitializedObject(type);
            });
        }

        public static string For<TController>(this UrlHelper url, Func<TController, string> getUrl)
            where TController : Controller
        {
            return getUrl(InstanceOf<TController>());
        }

        public static string GetUrl(this UrlHelper url, Func<ActionResult> action)
        {
            return GetUrlImpl(url, action, Array.Empty<object>());
        }

        public static string GetUrl<T1>(this UrlHelper urlHelper, Func<T1, ActionResult> action, T1 arg1)
        {
            return GetUrlImpl(urlHelper, action, arg1);
        }

        public static string Action<T1, T2>(this UrlHelper urlHelper, Func<T1, T2, ActionResult> action, T1 arg1, T2 arg2)
        {
            return GetUrlImpl(urlHelper, action, arg1, arg2);
        }

        public static string Action<T1, T2, T3>(this UrlHelper urlHelper, Func<T1, T2, T3, ActionResult> action, T1 arg1, T2 arg2, T3 arg3)
        {
            return GetUrlImpl(urlHelper, action, arg1, arg2, arg3);
        }

        public static string Action<T1, T2, T3, T4>(this UrlHelper urlHelper, Func<T1, T2, T3, T4, ActionResult> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            return GetUrlImpl(urlHelper, action, arg1, arg2, arg3, arg4);
        }

        public static string Action<T1, T2, T3, T4, T5>(this UrlHelper urlHelper, Func<T1, T2, T3, T4, T5, ActionResult> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            return GetUrlImpl(urlHelper, action, arg1, arg2, arg3, arg4, arg5);
        }

        public static string Action<T1, T2, T3, T4, T5, T6>(this UrlHelper urlHelper, Func<T1, T2, T3, T4, T5, T6, ActionResult> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
        {
            return GetUrlImpl(urlHelper, action, arg1, arg2, arg3, arg4, arg5, arg6);
        }

        private static string GetUrlImpl(UrlHelper urlHelper, Delegate action, params object[] args)
        {
            var routeValues = new RouteValueDictionary();

            if (args != null && args.Length > 0)
            {
                routeValues = new RouteValueDictionary();
                var parameters = action.Method.GetParameters();
                for (int i = 0; i < parameters.Length; i++)
                {
                    routeValues[parameters[i].Name] = args[i];
                }
            }

            var actionName = actionNames.GetOrAdd(action.Method, method =>
            {
                var attr = method.GetCustomAttribute<ActionNameAttribute>(true);
                return (attr != null) ? attr.Name : method.Name;
            });

            var controllerName = controllerNames.GetOrAdd(action.Method, method =>
            {
                return action.Method.DeclaringType.Name;
            });

            routeValues["action"] = actionName;
            routeValues["controller"] = controllerName.Substring(0, controllerName.LastIndexOf("Controller"));

            return urlHelper.RouteUrl(routeValues);
        }
    }
}