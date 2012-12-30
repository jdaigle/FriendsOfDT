﻿// <auto-generated />
// This file was generated by a T4 template.
// Don't change it directly as your change would get overwritten.  Instead, make changes
// to the .tt file (i.e. the T4 template) and save it to regenerate this file.

// Make sure the compiler doesn't complain about missing Xml comments
#pragma warning disable 1591
#region T4MVC

using System;
using System.Diagnostics;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using System.Web.Mvc.Html;
using System.Web.Routing;
using T4MVC;

[GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
public static class MVC {
    public static FODT.Controllers.PeopleController People = new FODT.Controllers.T4MVC_PeopleController();
    public static FODT.Controllers.ShowsController Shows = new FODT.Controllers.T4MVC_ShowsController();
    public static T4MVC.SharedController Shared = new T4MVC.SharedController();
}

namespace T4MVC {
}

   
namespace System.Web.Mvc {
    [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
    public static class T4Extensions {
        public static MvcHtmlString ActionLink(this HtmlHelper htmlHelper, string linkText, ActionResult result) {
            return htmlHelper.ActionLink(linkText, result, null, null, null, null);
        }

        public static MvcHtmlString ActionLink(this HtmlHelper htmlHelper, string linkText, ActionResult result, object htmlAttributes, string protocol = null, string hostName = null, string fragment = null) {
            return htmlHelper.RouteLink(linkText, null, protocol ?? result.GetT4MVCResult().Protocol, hostName, fragment, result.GetRouteValueDictionary(), HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        public static MvcHtmlString ActionLink(this HtmlHelper htmlHelper, string linkText, ActionResult result, IDictionary<string, object> htmlAttributes, string protocol = null, string hostName = null, string fragment = null) {
            return htmlHelper.RouteLink(linkText, null, protocol ?? result.GetT4MVCResult().Protocol, hostName, fragment, result.GetRouteValueDictionary(), htmlAttributes);
        }

        public static MvcForm BeginForm(this HtmlHelper htmlHelper, ActionResult result) {
            return htmlHelper.BeginForm(result, FormMethod.Post);
        }

        public static MvcForm BeginForm(this HtmlHelper htmlHelper, ActionResult result, FormMethod formMethod) {
            return htmlHelper.BeginForm(result, formMethod, null);
        }

        public static MvcForm BeginForm(this HtmlHelper htmlHelper, ActionResult result, FormMethod formMethod, object htmlAttributes) {
            return BeginForm(htmlHelper, result, formMethod, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        public static MvcForm BeginForm(this HtmlHelper htmlHelper, ActionResult result, FormMethod formMethod, IDictionary<string, object> htmlAttributes) {
            var callInfo = result.GetT4MVCResult();
            return htmlHelper.BeginForm(callInfo.Action, callInfo.Controller, callInfo.RouteValueDictionary, formMethod, htmlAttributes);
        }

        public static void RenderAction(this HtmlHelper htmlHelper, ActionResult result) {
            var callInfo = result.GetT4MVCResult();
            htmlHelper.RenderAction(callInfo.Action, callInfo.Controller, callInfo.RouteValueDictionary);
        }

        public static MvcHtmlString Action(this HtmlHelper htmlHelper, ActionResult result) {
            var callInfo = result.GetT4MVCResult();
            return htmlHelper.Action(callInfo.Action, callInfo.Controller, callInfo.RouteValueDictionary);
        }

        public static string Action(this UrlHelper urlHelper, ActionResult result) {
            return urlHelper.Action(result, null, null);
        }

        public static string Action(this UrlHelper urlHelper, ActionResult result, string protocol = null, string hostName = null) {
            return urlHelper.RouteUrl(null, result.GetRouteValueDictionary(), protocol ?? result.GetT4MVCResult().Protocol, hostName);
        }

        public static string ActionAbsolute(this UrlHelper urlHelper, ActionResult result) {
            return string.Format("{0}{1}",urlHelper.RequestContext.HttpContext.Request.Url.GetLeftPart(UriPartial.Authority),
                urlHelper.RouteUrl(result.GetRouteValueDictionary()));
        }

        public static MvcHtmlString ActionLink(this AjaxHelper ajaxHelper, string linkText, ActionResult result, AjaxOptions ajaxOptions) {
            return ajaxHelper.RouteLink(linkText, result.GetRouteValueDictionary(), ajaxOptions);
        }

        public static MvcHtmlString ActionLink(this AjaxHelper ajaxHelper, string linkText, ActionResult result, AjaxOptions ajaxOptions, object htmlAttributes) {
            return ajaxHelper.RouteLink(linkText, result.GetRouteValueDictionary(), ajaxOptions, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        public static MvcHtmlString ActionLink(this AjaxHelper ajaxHelper, string linkText, ActionResult result, AjaxOptions ajaxOptions, IDictionary<string, object> htmlAttributes) {
            return ajaxHelper.RouteLink(linkText, result.GetRouteValueDictionary(), ajaxOptions, htmlAttributes);
        }

        public static MvcForm BeginForm(this AjaxHelper ajaxHelper, ActionResult result, AjaxOptions ajaxOptions) {
            return ajaxHelper.BeginForm(result, ajaxOptions, null);
        }

        public static MvcForm BeginForm(this AjaxHelper ajaxHelper, ActionResult result, AjaxOptions ajaxOptions, object htmlAttributes) {
            return BeginForm(ajaxHelper, result, ajaxOptions, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        public static MvcForm BeginForm(this AjaxHelper ajaxHelper, ActionResult result, AjaxOptions ajaxOptions, IDictionary<string, object> htmlAttributes) {
            var callInfo = result.GetT4MVCResult();
            return ajaxHelper.BeginForm(callInfo.Action, callInfo.Controller, callInfo.RouteValueDictionary, ajaxOptions, htmlAttributes);
        }

        public static Route MapRoute(this RouteCollection routes, string name, string url, ActionResult result) {
            return MapRoute(routes, name, url, result, null /*namespaces*/);
        }

        public static Route MapRoute(this RouteCollection routes, string name, string url, ActionResult result, object defaults) {
            return MapRoute(routes, name, url, result, defaults, null /*constraints*/, null /*namespaces*/);
        }

        public static Route MapRoute(this RouteCollection routes, string name, string url, ActionResult result, string[] namespaces) {
            return MapRoute(routes, name, url, result, null /*defaults*/, namespaces);
        }

        public static Route MapRoute(this RouteCollection routes, string name, string url, ActionResult result, object defaults, object constraints) {
            return MapRoute(routes, name, url, result, defaults, constraints, null /*namespaces*/);
        }

        public static Route MapRoute(this RouteCollection routes, string name, string url, ActionResult result, object defaults, string[] namespaces) {
            return MapRoute(routes, name, url, result, defaults, null /*constraints*/, namespaces);
        }

        public static Route MapRoute(this RouteCollection routes, string name, string url, ActionResult result, object defaults, object constraints, string[] namespaces) {
            // Create and add the route
            var route = CreateRoute(url, result, defaults, constraints, namespaces);
            routes.Add(name, route);
            return route;
        }

        // Note: can't name the AreaRegistrationContext methods 'MapRoute', as that conflicts with the existing methods
        public static Route MapRouteArea(this AreaRegistrationContext context, string name, string url, ActionResult result) {
            return MapRouteArea(context, name, url, result, null /*namespaces*/);
        }

        public static Route MapRouteArea(this AreaRegistrationContext context, string name, string url, ActionResult result, object defaults) {
            return MapRouteArea(context, name, url, result, defaults, null /*constraints*/, null /*namespaces*/);
        }

        public static Route MapRouteArea(this AreaRegistrationContext context, string name, string url, ActionResult result, string[] namespaces) {
            return MapRouteArea(context, name, url, result, null /*defaults*/, namespaces);
        }

        public static Route MapRouteArea(this AreaRegistrationContext context, string name, string url, ActionResult result, object defaults, object constraints) {
            return MapRouteArea(context, name, url, result, defaults, constraints, null /*namespaces*/);
        }

        public static Route MapRouteArea(this AreaRegistrationContext context, string name, string url, ActionResult result, object defaults, string[] namespaces) {
            return MapRouteArea(context, name, url, result, defaults, null /*constraints*/, namespaces);
        }

        public static Route MapRouteArea(this AreaRegistrationContext context, string name, string url, ActionResult result, object defaults, object constraints, string[] namespaces) {
            // Create and add the route
            if ((namespaces == null) && (context.Namespaces != null)) {
                 namespaces = context.Namespaces.ToArray();
            }
            var route = CreateRoute(url, result, defaults, constraints, namespaces);
            context.Routes.Add(name, route);
            route.DataTokens["area"] = context.AreaName;
            bool useNamespaceFallback = (namespaces == null) || (namespaces.Length == 0);
            route.DataTokens["UseNamespaceFallback"] = useNamespaceFallback;
            return route;
        }

        private static Route CreateRoute(string url, ActionResult result, object defaults, object constraints, string[] namespaces) {
            // Start by adding the default values from the anonymous object (if any)
            var routeValues = new RouteValueDictionary(defaults);

            // Then add the Controller/Action names and the parameters from the call
            foreach (var pair in result.GetRouteValueDictionary()) {
                routeValues.Add(pair.Key, pair.Value);
            }

            var routeConstraints = new RouteValueDictionary(constraints);

            // Create and add the route
            var route = new Route(url, routeValues, routeConstraints, new MvcRouteHandler());

            route.DataTokens = new RouteValueDictionary();

            if (namespaces != null && namespaces.Length > 0) {
                route.DataTokens["Namespaces"] = namespaces;
            }

            return route;
        }

        public static IT4MVCActionResult GetT4MVCResult(this ActionResult result) {
            var t4MVCResult = result as IT4MVCActionResult;
            if (t4MVCResult == null) {
                throw new InvalidOperationException("T4MVC was called incorrectly. You may need to force it to regenerate by right clicking on T4MVC.tt and choosing Run Custom Tool");
            }
            return t4MVCResult;
        }

        public static RouteValueDictionary GetRouteValueDictionary(this ActionResult result) {
            return result.GetT4MVCResult().RouteValueDictionary;
        }

        public static ActionResult AddRouteValues(this ActionResult result, object routeValues) {
            return result.AddRouteValues(new RouteValueDictionary(routeValues));
        }

        public static ActionResult AddRouteValues(this ActionResult result, RouteValueDictionary routeValues) {
            RouteValueDictionary currentRouteValues = result.GetRouteValueDictionary();

            // Add all the extra values
            foreach (var pair in routeValues) {
                ModelUnbinderHelpers.AddRouteValues(currentRouteValues, pair.Key, pair.Value);
            }

            return result;
        }

        public static ActionResult AddRouteValues(this ActionResult result, System.Collections.Specialized.NameValueCollection nameValueCollection) {
            // Copy all the values from the NameValueCollection into the route dictionary
            nameValueCollection.CopyTo(result.GetRouteValueDictionary());
            return result;
        }

        public static ActionResult AddRouteValue(this ActionResult result, string name, object value) {
            RouteValueDictionary routeValues = result.GetRouteValueDictionary();
            ModelUnbinderHelpers.AddRouteValues(routeValues, name, value);
            return result;
        }
        
        public static void InitMVCT4Result(this IT4MVCActionResult result, string area, string controller, string action, string protocol = null) {
            result.Controller = controller;
            result.Action = action;
            result.Protocol = T4MVCHelpers.IsProduction() ? protocol : null;
            result.RouteValueDictionary = new RouteValueDictionary();
            result.RouteValueDictionary.Add("Area", area ?? "");
            result.RouteValueDictionary.Add("Controller", controller);
            result.RouteValueDictionary.Add("Action", action);
        }

        public static bool FileExists(string virtualPath) {
            if (!HostingEnvironment.IsHosted) return false;
            string filePath = HostingEnvironment.MapPath(virtualPath);
            return System.IO.File.Exists(filePath);
        }

        static DateTime CenturyBegin=new DateTime(2001,1,1);
        public static string TimestampString(string virtualPath) {
            if (!HostingEnvironment.IsHosted) return string.Empty;
            string filePath = HostingEnvironment.MapPath(virtualPath);
            return Convert.ToString((System.IO.File.GetLastWriteTimeUtc(filePath).Ticks-CenturyBegin.Ticks)/1000000000,16);            
        }
    }
}



namespace T4MVC {
    [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
    public class Dummy {
        private Dummy() { }
        public static Dummy Instance = new Dummy();
    }
}


  

   
[GeneratedCode("T4MVC", "2.0")]   
public interface IT4MVCActionResult {   
    string Action { get; set; }   
    string Controller { get; set; }   
    RouteValueDictionary RouteValueDictionary { get; set; } 
    string Protocol {get; set; }  
}   
  

[GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
public class T4MVC_ActionResult : System.Web.Mvc.ActionResult, IT4MVCActionResult {
    public T4MVC_ActionResult(string area, string controller, string action, string protocol = null): base()  {
        this.InitMVCT4Result(area, controller, action, protocol);
    }
     
    public override void ExecuteResult(System.Web.Mvc.ControllerContext context) { }
    
    public string Controller { get; set; }
    public string Action { get; set; }
    public string Protocol { get; set; }
    public RouteValueDictionary RouteValueDictionary { get; set; }
}



namespace Links {
}

public static class T4MVCHelpers {
    // You can change the ProcessVirtualPath method to modify the path that gets returned to the client.
    // e.g. you can prepend a domain, or append a query string:
    //      return "http://localhost" + path + "?foo=bar";
    private static string ProcessVirtualPathDefault(string virtualPath) {
        // The path that comes in starts with ~/ and must first be made absolute
        string path = VirtualPathUtility.ToAbsolute(virtualPath);
        
        // Add your own modifications here before returning the path
        return path;
    }

    // Calling ProcessVirtualPath through delegate to allow it to be replaced for unit testing
    public static Func<string, string> ProcessVirtualPath = ProcessVirtualPathDefault;


    // Logic to determine if the app is running in production or dev environment
    public static bool IsProduction() { 
        return (HttpContext.Current != null && !HttpContext.Current.IsDebuggingEnabled); 
    }
}


namespace FODT.Controllers {
    public partial class PeopleController {
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public PeopleController() { }

        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        protected PeopleController(Dummy d) { }

        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        protected RedirectToRouteResult RedirectToAction(ActionResult result) {
            var callInfo = result.GetT4MVCResult();
            return RedirectToRoute(callInfo.RouteValueDictionary);
        }

        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        protected RedirectToRouteResult RedirectToActionPermanent(ActionResult result) {
            var callInfo = result.GetT4MVCResult();
            return RedirectToRoutePermanent(callInfo.RouteValueDictionary);
        }

        [NonAction]
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public System.Web.Mvc.ActionResult Display() {
            return new T4MVC_ActionResult(Area, Name, ActionNames.Display);
        }
        [NonAction]
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public System.Web.Mvc.ActionResult EditBiography() {
            return new T4MVC_ActionResult(Area, Name, ActionNames.EditBiography);
        }
        [NonAction]
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public System.Web.Mvc.ActionResult SaveEditBiography() {
            return new T4MVC_ActionResult(Area, Name, ActionNames.SaveEditBiography);
        }

        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public PeopleController Actions { get { return MVC.People; } }
        [GeneratedCode("T4MVC", "2.0")]
        public readonly string Area = "";
        [GeneratedCode("T4MVC", "2.0")]
        public readonly string Name = "People";
        [GeneratedCode("T4MVC", "2.0")]
        public const string NameConst = "People";

        static readonly ActionNamesClass s_actions = new ActionNamesClass();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionNamesClass ActionNames { get { return s_actions; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionNamesClass {
            public readonly string Display = "Display";
            public readonly string EditBiography = "EditBiography";
            public readonly string SaveEditBiography = "SaveEditBiography";
        }

        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionNameConstants {
            public const string Display = "Display";
            public const string EditBiography = "EditBiography";
            public const string SaveEditBiography = "SaveEditBiography";
        }


        static readonly ActionParamsClass_Display s_params_Display = new ActionParamsClass_Display();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionParamsClass_Display DisplayParams { get { return s_params_Display; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionParamsClass_Display {
            public readonly string personId = "personId";
        }
        static readonly ActionParamsClass_EditBiography s_params_EditBiography = new ActionParamsClass_EditBiography();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionParamsClass_EditBiography EditBiographyParams { get { return s_params_EditBiography; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionParamsClass_EditBiography {
            public readonly string personId = "personId";
        }
        static readonly ActionParamsClass_SaveEditBiography s_params_SaveEditBiography = new ActionParamsClass_SaveEditBiography();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionParamsClass_SaveEditBiography SaveEditBiographyParams { get { return s_params_SaveEditBiography; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionParamsClass_SaveEditBiography {
            public readonly string personId = "personId";
            public readonly string model = "model";
        }
        static readonly ViewNames s_views = new ViewNames();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ViewNames Views { get { return s_views; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ViewNames {
            public readonly string Display = "~/Views/People/Display.cshtml";
            public readonly string DisplayViewModel = "~/Views/People/DisplayViewModel.cs";
            public readonly string EditBiography = "~/Views/People/EditBiography.cshtml";
            public readonly string EditBiographyViewModel = "~/Views/People/EditBiographyViewModel.cs";
        }
    }

    [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
    public class T4MVC_PeopleController: FODT.Controllers.PeopleController {
        public T4MVC_PeopleController() : base(Dummy.Instance) { }

        public override System.Web.Mvc.ActionResult Display(int personId) {
            var callInfo = new T4MVC_ActionResult(Area, Name, ActionNames.Display);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "personId", personId);
            return callInfo;
        }

        public override System.Web.Mvc.ActionResult EditBiography(int personId) {
            var callInfo = new T4MVC_ActionResult(Area, Name, ActionNames.EditBiography);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "personId", personId);
            return callInfo;
        }

        public override System.Web.Mvc.ActionResult SaveEditBiography(int personId, FODT.Controllers.PeopleController.SaveEditBiographyModel model) {
            var callInfo = new T4MVC_ActionResult(Area, Name, ActionNames.SaveEditBiography);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "personId", personId);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "model", model);
            return callInfo;
        }

    }
}

namespace FODT.Controllers {
    public partial class ShowsController {
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ShowsController() { }

        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        protected ShowsController(Dummy d) { }

        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        protected RedirectToRouteResult RedirectToAction(ActionResult result) {
            var callInfo = result.GetT4MVCResult();
            return RedirectToRoute(callInfo.RouteValueDictionary);
        }

        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        protected RedirectToRouteResult RedirectToActionPermanent(ActionResult result) {
            var callInfo = result.GetT4MVCResult();
            return RedirectToRoutePermanent(callInfo.RouteValueDictionary);
        }

        [NonAction]
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public System.Web.Mvc.ActionResult Display() {
            return new T4MVC_ActionResult(Area, Name, ActionNames.Display);
        }

        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ShowsController Actions { get { return MVC.Shows; } }
        [GeneratedCode("T4MVC", "2.0")]
        public readonly string Area = "";
        [GeneratedCode("T4MVC", "2.0")]
        public readonly string Name = "Shows";
        [GeneratedCode("T4MVC", "2.0")]
        public const string NameConst = "Shows";

        static readonly ActionNamesClass s_actions = new ActionNamesClass();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionNamesClass ActionNames { get { return s_actions; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionNamesClass {
            public readonly string Display = "Display";
        }

        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionNameConstants {
            public const string Display = "Display";
        }


        static readonly ActionParamsClass_Display s_params_Display = new ActionParamsClass_Display();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionParamsClass_Display DisplayParams { get { return s_params_Display; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionParamsClass_Display {
            public readonly string showId = "showId";
        }
        static readonly ViewNames s_views = new ViewNames();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ViewNames Views { get { return s_views; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ViewNames {
            public readonly string Display = "~/Views/Shows/Display.cshtml";
            public readonly string DisplayViewModel = "~/Views/Shows/DisplayViewModel.cs";
        }
    }

    [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
    public class T4MVC_ShowsController: FODT.Controllers.ShowsController {
        public T4MVC_ShowsController() : base(Dummy.Instance) { }

        public override System.Web.Mvc.ActionResult Display(int showId) {
            var callInfo = new T4MVC_ActionResult(Area, Name, ActionNames.Display);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "showId", showId);
            return callInfo;
        }

    }
}

namespace T4MVC {
    public class SharedController {

        static readonly ViewNames s_views = new ViewNames();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ViewNames Views { get { return s_views; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ViewNames {
            public readonly string _Layout = "~/Views/Shared/_Layout.cshtml";
        }
    }

}




#endregion T4MVC
#pragma warning restore 1591

namespace System.Web.Mvc {
    #region ModelUnbinders
    [GeneratedCode("T4MVC", "2.0")]
    public interface IModelUnbinder {
        void UnbindModel(RouteValueDictionary routeValueDictionary, string routeName, object routeValue);
    }
    [GeneratedCode("T4MVC", "2.0")]
    public interface IModelUnbinder<in T> where T : class {
        void UnbindModel(RouteValueDictionary routeValueDictionary, string routeName, T routeValue);
    }
    [GeneratedCode("T4MVC", "2.0")]
        public class ModelUnbinders {
        private class GenericModelUnbinderWrapper<T> : IModelUnbinder where T : class {
            private readonly IModelUnbinder<T> _unbinder;

            public GenericModelUnbinderWrapper(IModelUnbinder<T> unbinder) {
                _unbinder = unbinder;
            }

            public void UnbindModel(RouteValueDictionary routeValueDictionary, string routeName, object routeValue) {
                var typedObject = routeValue as T;
                _unbinder.UnbindModel(routeValueDictionary, routeName, typedObject);
            }
        }
        
        private readonly Dictionary<Type, IModelUnbinder> _unbinders = new Dictionary<Type, IModelUnbinder>();
        public virtual void Add(Type type, IModelUnbinder unbinder) {
            _unbinders[type] = unbinder;
        }
        public virtual void Add<T>(IModelUnbinder<T> unbinder) where T : class {
            Add(typeof(T), new GenericModelUnbinderWrapper<T>(unbinder));
        }
        public virtual IModelUnbinder FindUnbinderFor(Type type) {
            IModelUnbinder resultUnbinder = null;
            Type baseType = null;
            foreach (var unbinder in _unbinders) {
                if (unbinder.Key.IsAssignableFrom(type)) {
                    if ((baseType == null) || baseType.IsAssignableFrom(unbinder.Key)) {
                        resultUnbinder = unbinder.Value;
                        baseType = unbinder.Key;
                    }
                }
            }
            return resultUnbinder;
        }
        
        public virtual void Clear() {
            _unbinders.Clear();
        }
    }
    [GeneratedCode("T4MVC", "2.0")]
    public class DefaultModelUnbinder : IModelUnbinder {
        public void UnbindModel(RouteValueDictionary routeValueDictionary, string routeName, object routeValue) {
            routeValueDictionary.Add(routeName, routeValue);
        }
    }
    [GeneratedCode("T4MVC", "2.0")]
    public class PropertiesUnbinder : IModelUnbinder {
        public virtual void UnbindModel(RouteValueDictionary routeValueDictionary, string routeName, object routeValue) {
            var dict = new RouteValueDictionary(routeValue);
            foreach (var entry in dict) {
                var name = entry.Key;

                if (!(entry.Value is string) && (entry.Value is System.Collections.IEnumerable)) {
                    var enumerableValue = (System.Collections.IEnumerable)entry.Value;
                    var i = 0;
                    foreach (var enumerableElement in enumerableValue) {
                        ModelUnbinderHelpers.AddRouteValues(routeValueDictionary, string.Format("{0}.{1}[{2}]", routeName, name, i), enumerableElement);
                        i++;
                    }
                }
                else {
                    ModelUnbinderHelpers.AddRouteValues(routeValueDictionary, routeName + "." + name, entry.Value);
                }
            }
        }
    }
    public class ModelUnbinderHelpers {
        public static void AddRouteValues(RouteValueDictionary routeValueDictionary, string routeName, object routeValue) {
            IModelUnbinder unbinder = DefaultModelUnbinder;
            if (routeValue != null)
            {
                unbinder = ModelUnbinders.FindUnbinderFor(routeValue.GetType()) ?? DefaultModelUnbinder;
            }
            unbinder.UnbindModel(routeValueDictionary, routeName, routeValue);
        }
    
        private static readonly ModelUnbinders _modelUnbinders = new ModelUnbinders();
        public static ModelUnbinders ModelUnbinders {
            get { return _modelUnbinders; }
        }

        public static IModelUnbinder DefaultModelUnbinder { get; set; }
        static ModelUnbinderHelpers() {
            DefaultModelUnbinder = new DefaultModelUnbinder();
        }
    }
}
    #endregion



