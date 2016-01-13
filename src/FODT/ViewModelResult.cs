using System.Net;
using System.Web.Mvc;

namespace FODT
{
    public class ViewModelResult : ActionResult
    {
        public ViewModelResult(object viewModel)
        {
            ViewModel = viewModel;
        }

        public HttpStatusCode? StatusCode
        {
            get
            {
                if (ViewModel is HttpApiResult)
                {
                    return ((HttpApiResult)ViewModel).HttpStatusCode;
                }
                return null;
            }
        }
        public object ViewModel { get; }

        public override void ExecuteResult(ControllerContext context)
        {
            if (context.HttpContext.Request.IsAjaxRequest())
            {
                GetAjaxActionResult(context).ExecuteResult(context);
                
            }
            else
            {
                GetNonAjaxActionResult(context).ExecuteResult(context);
            }
        }

        private ActionResult GetNonAjaxActionResult(ControllerContext context)
        {
            if (ViewModel is HttpApiResult)
            {
                return new RedirectResult(((HttpApiResult)ViewModel).RedirectToURL);
            }
            context.Controller.ViewData.Model = ViewModel;
            return new ViewResult()
            {
                ViewName = null,
                MasterName = null,
                ViewData = context.Controller.ViewData,
                TempData = context.Controller.TempData,
                ViewEngineCollection = ViewEngines.Engines,
            };
        }

        private ActionResult GetAjaxActionResult(ControllerContext context)
        {
            if (context.HttpContext.Request.Accepts("application/json", "text/javascript"))
            {
                return new JsonSerializerResult()
                {
                    StatusCode = this.StatusCode,
                    //ContentType = "application/json",
                    Data = ViewModel,
                };
            }
            if (context.HttpContext.Request.Accepts("application/xml", "text/xml"))
            {
                return new HttpBadRequestResult("XML Not Supported");
            }
            context.Controller.ViewData.Model = ViewModel;
            return new PartialViewResult()
            {
                ViewName = null,
                ViewData = context.Controller.ViewData,
                TempData = context.Controller.TempData,
                ViewEngineCollection = ViewEngines.Engines,
            };
        }
    }
}