using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FODT.Models.IMDT;

namespace FODT.Security
{
    public static class AuthorizationContextExtensions
    {
        public static bool CanEditPhoto(this ControllerContext controllerContext, Photo photo)
        {
#if DEBUG
            if (!controllerContext.HttpContext.Request.Params["edit"].IsNullOrWhiteSpace())
            {
                return true;
            }
#endif
            if (controllerContext.HttpContext.User == null ||
                !controllerContext.HttpContext.User.Identity.IsAuthenticated)
            {
                return false;
            }

            return controllerContext.HttpContext.User.IsInRole(RoleNames.Archivist);
        }

        public static bool CanEditPerson(this ControllerContext controllerContext, Person person)
        {
#if DEBUG
            if (!controllerContext.HttpContext.Request.Params["edit"].IsNullOrWhiteSpace())
            {
                return true;
            }
#endif
            if (controllerContext.HttpContext.User == null ||
                !controllerContext.HttpContext.User.Identity.IsAuthenticated)
            {
                return false;
            }

            return controllerContext.HttpContext.User.IsInRole(RoleNames.Archivist);
        }

        public static bool CanEditShow(this ControllerContext controllerContext, Show show)
        {
#if DEBUG
            if (!controllerContext.HttpContext.Request.Params["edit"].IsNullOrWhiteSpace())
            {
                return true;
            }
#endif
            if (controllerContext.HttpContext.User == null ||
                !controllerContext.HttpContext.User.Identity.IsAuthenticated)
            {
                return false;
            }

            return controllerContext.HttpContext.User.IsInRole(RoleNames.Archivist);
        }
    }
}