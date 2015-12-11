using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
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
            var isHTTPGET = filterContext.HttpContext.Request.HttpMethod.Equals("GET", StringComparison.InvariantCultureIgnoreCase);
            databaseSession = DatabaseBootstrapper.SessionFactory.OpenSession();
            databaseSession.FlushMode = FlushMode.Commit;
            databaseSession.Transaction.Begin(isHTTPGET ? IsolationLevel.ReadCommitted : IsolationLevel.RepeatableRead);
            databaseSession.DefaultReadOnly = isHTTPGET;
            databaseSessionClosed = false;
            base.OnActionExecuting(filterContext);
        }

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            var isHTTPGET = filterContext.HttpContext.Request.HttpMethod.Equals("GET", StringComparison.InvariantCultureIgnoreCase);
            if (databaseSession != null && filterContext.Exception == null)
            {
                try
                {
                    using (databaseSession)
                    {
                        AssertDatabaseSessionNotDirty(isHTTPGET);
                        if (isHTTPGET)
                        {
                            databaseSession.Clear(); // clear so we don't check dirty and flush
                        }
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

        [Conditional("DEBUG")]
        private void AssertDatabaseSessionNotDirty(bool isHTTPGET)
        {
            if (isHTTPGET && databaseSession.IsDirty())
            {
                Debug.Fail("Executed an HTTP GET, but the database UoW has unflushed changes. This is probably a bug.");
            }
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