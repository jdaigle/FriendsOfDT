using System;
using System.Reflection;
using System.Web.Caching;
using System.Web.Mvc;
using RazorGenerator.Mvc;

namespace FODT.Infrastructure
{
    public class ViewModelSpecifiedViewEngine : PrecompiledMvcEngine
    {
        private readonly RazorViewEngine razorViewEngine;

        public ViewModelSpecifiedViewEngine()
            : base(typeof(MvcApplication).Assembly)
        {
            razorViewEngine = new RazorViewEngine();
#if DEBUG
            UsePhysicalViewsIfNewer = true;
            PreemptPhysicalFiles = false;
#else
            UsePhysicalViewsIfNewer = false;
            PreemptPhysicalFiles = true;
#endif
        }

        public override ViewEngineResult FindPartialView(ControllerContext controllerContext, string partialViewName, bool useCache)
        {
            var viewModel = controllerContext.Controller.ViewData.Model;
            if (controllerContext is ViewContext)
            {
                viewModel = ((ViewContext)controllerContext).ViewData.Model;
            }
            partialViewName = TryFindViewFromViewModel(controllerContext.HttpContext.Cache, viewModel) ?? partialViewName;
            var result = base.FindPartialView(controllerContext, partialViewName, useCache);
            if (result.View == null && !PreemptPhysicalFiles)
            {
                result = razorViewEngine.FindPartialView(controllerContext, partialViewName, useCache);
            }
            return result;
        }

        public override ViewEngineResult FindView(ControllerContext controllerContext, string viewName, string masterName, bool useCache)
        {
            var viewModel = controllerContext.Controller.ViewData.Model;
            if (controllerContext is ViewContext)
            {
                viewModel = ((ViewContext)controllerContext).ViewData.Model;
            }
            viewName = TryFindViewFromViewModel(controllerContext.HttpContext.Cache, viewModel) ?? viewName;
            var result = base.FindView(controllerContext, viewName, masterName, useCache);
            if (result.View == null && !PreemptPhysicalFiles)
            {
                result = razorViewEngine.FindView(controllerContext, viewName, masterName, useCache);
            }
            return result;
        }

        protected string TryFindViewFromViewModel(Cache cache, object viewModel)
        {
            if (viewModel != null)
            {
                var viewModelType = viewModel.GetType();
                var cacheKey = "ViewModelViewName_" + viewModelType.FullName;
                var cachedValue = (string)cache.Get(cacheKey);
                if (cachedValue != null)
                {
                    return cachedValue != NoVirtualPathCacheValue ? cachedValue : null;
                }
                while (viewModelType != typeof(object))
                {
                    var viewModelName = viewModelType.Name;
                    var namespacePart = viewModelType.Namespace.Substring("FODT.".Length);
                    var virtualPath = "~/" + namespacePart.Replace(".", "/") + "/" + viewModelName.Replace("ViewModel", "") + ".cshtml";
                    if (Exists(virtualPath) || VirtualPathProvider.FileExists(virtualPath))
                    {
                        cache.Insert(cacheKey, virtualPath, null /* dependencies */, Cache.NoAbsoluteExpiration, _defaultCacheTimeSpan);
                        return virtualPath;
                    }
                    viewModelType = viewModelType.BaseType;
                }

                // no view found
                cache.Insert(cacheKey, NoVirtualPathCacheValue, null /* dependencies */, Cache.NoAbsoluteExpiration, _defaultCacheTimeSpan);
            }
            return null;
        }

        private const string NoVirtualPathCacheValue = "NULL";
        private static readonly TimeSpan _defaultCacheTimeSpan = new TimeSpan(1, 0, 0);
    }
}