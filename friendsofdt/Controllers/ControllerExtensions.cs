
using System.Web.Mvc;
namespace FriendsOfDT.Controllers {
    public static class ControllerExtensions {
        public static RenderJsonResult RenderJsonErrorCode(this IController controller, int errorCode, string message) {
            return RenderJsonResult.Render(new { ErrorCode = errorCode, Message = message ?? string.Empty });
        }

        public static RenderJsonResult RenderJsonSuccessErrorCode(this IController controller) {
            return RenderJsonResult.Render(new { ErrorCode = 0, Message = "Sucesss" });
        }
    }
}