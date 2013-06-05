using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FODT
{
    public class LegacyUrlFilter : FilterAttribute, IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.Result != null)
            {
                return;
            }
            try
            {
                var param = filterContext.HttpContext.Request.Params;
                var action = param["action"];
                if (!string.IsNullOrWhiteSpace(action))
                {
                    switch (action.ToLowerInvariant())
                    {
                        case "peep_detail":
                            filterContext.Result = RedirectToAction(MVC.Person.Get(int.Parse(param["peep_id"])));
                            break;
                        case "show_detail":
                            filterContext.Result = RedirectToAction(MVC.Person.Get(int.Parse(param["show_id"])));
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(e);
            }
        }

        private RedirectToRouteResult RedirectToAction(ActionResult result)
        {
            var callInfo = result.GetT4MVCResult();
            return new RedirectToRouteResult(null, callInfo.RouteValueDictionary, true);
        }

        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
            //no-op
        }
    }
}