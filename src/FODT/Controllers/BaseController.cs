using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using FODT.Database;
using FODT.Models.IMDT;
using NHibernate;

namespace FODT.Controllers
{
    public abstract class BaseController : Controller
    {
        private ISession databaseSession;
        private bool databaseSessionClosed;
        protected ISession DatabaseSession
        {
            get
            {
                if (databaseSessionClosed)
                {
                    throw new InvalidOperationException("The database Session has been closed. You should not execute database statements except in a Controller Action.");
                }
                return databaseSession;
            }
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            this.databaseSession = DatabaseBootstrapper.SessionFactory.OpenSession();
            this.databaseSession.FlushMode = FlushMode.Commit;
            this.databaseSession.Transaction.Begin();
            databaseSessionClosed = false;
            base.OnActionExecuting(filterContext);
        }

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (databaseSession != null)
            {
                try
                {
                    using (databaseSession)
                    {
                        if (databaseSession.Transaction.IsActive)
                        {
                            if (filterContext.Exception != null)
                            {
                                databaseSession.RollbackTransaction();
                            }
                            else
                            {
                                databaseSession.CommitTransaction();
                            }
                        }
                        databaseSession.Close();
                    }
                }
                finally
                {
                    databaseSession = null;
                    databaseSessionClosed = true;
                }
            }
            base.OnActionExecuted(filterContext);

            ViewBag.IsAuthenticated = this.User.Identity.IsAuthenticated;
            ViewBag.UserName = this.User.Identity.Name;
        }

        protected override void OnException(ExceptionContext filterContext)
        {
            if (databaseSession != null)
            {
                try
                {
                    using (databaseSession)
                    {
                        if (databaseSession.Transaction.IsActive)
                        {
                            databaseSession.RollbackTransaction();
                        }
                        databaseSession.Close();
                    }
                }
                finally
                {
                    databaseSession = null;
                    databaseSessionClosed = true;
                }
            }
            base.OnException(filterContext);
        }
    }
}