﻿using System.Web.Mvc;
using FriendsOfDT.Tasks;
using Raven.Client;

namespace FriendsOfDT.Controllers {
    public partial class AbstractController : Controller {
        public IDocumentSession DocumentSession { get; set; }

        protected override void OnActionExecuting(ActionExecutingContext filterContext) {
            DocumentSession = MvcApplication.DocumentStore.OpenSession();
        }

        protected override void OnActionExecuted(ActionExecutedContext filterContext) {
            ViewBag.IsAuthenticated = HttpContext.User.Identity.IsAuthenticated;
            ViewBag.EmailAddress = HttpContext.User.Identity.Name;
            try {
                using (DocumentSession) {
                    if (filterContext.Exception == null) {
                        DocumentSession.SaveChanges();
                        TaskExecuter.StartExecuting();
                    }
                }
            } finally {
                TaskExecuter.Discard();
            }
        }
    }
}