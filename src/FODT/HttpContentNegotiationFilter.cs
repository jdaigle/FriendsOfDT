using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FODT
{
    public class HttpContentNegotiationFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.HttpContext.Request.IsAjaxRequest())
            {
                // filter out bad requests
                if (filterContext.HttpContext.Request.Accepts("application/xml", "text/xml"))
                {
                    filterContext.Result = new HttpBadRequestResult("XML Not Supported");
                    return;
                }
            }
        }

        public void OnActionExecuted(ActionExecutedContext filterContext) { } // no op
    }
}