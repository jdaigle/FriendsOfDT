using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using FODT.Controllers;
using ExpressionHelper = Microsoft.Web.Mvc.Internal.ExpressionHelper;

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
                            filterContext.Result = Redirect<PersonController>(c => c.PersonDetails(int.Parse(param["peep_id"])));
                            break;
                        case "show_detail":
                            filterContext.Result = Redirect<PersonController>(c => c.PersonDetails(int.Parse(param["show_id"])));
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(e);
            }
        }

        private RedirectToRouteResult Redirect<TController>(Expression<Action<TController>> action) where TController : Controller
        {
            var routeValues = ExpressionHelper.GetRouteValuesFromExpression(action);
            return new RedirectToRouteResult(routeValues);
        }

        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
            //no-op
        }
    }
}