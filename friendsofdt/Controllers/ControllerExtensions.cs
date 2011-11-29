
using System.Web.Mvc;
namespace FriendsOfDT.Controllers {
    public static class ControllerExtensions {
        public static RenderJsonResult RenderJsonErrorCode(this IController controller, int errorCode, string message) {
            return RenderJsonResult.Render(new { errorCode = errorCode, message = message ?? string.Empty });
        }

        public static RenderJsonResult RenderJsonSuccessErrorCode(this IController controller) {
            return RenderJsonResult.Render(new { errorCode = 0, message = "Sucesss" });
        }
    }
}