using System.Web.Mvc;
using Newtonsoft.Json;

namespace FriendsOfDT {
    public class RenderJsonResult : ActionResult {

        public static RenderJsonResult Render(object data) {
            return new RenderJsonResult() { Data = data };
        }

        public object Data { get; set; }

        public override void ExecuteResult(ControllerContext context) {
            context.HttpContext.Response.ContentType = "application/json";

            var serializer = new JsonSerializer();
            serializer.Serialize(context.HttpContext.Response.Output, Data);
        }
    }
}
