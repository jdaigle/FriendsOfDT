using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using FODT.Database;
using FODT.Models.Entities;
using NHibernate;

namespace FODT.Controllers
{
    public abstract partial class BaseController : Controller
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
                            databaseSession.CommitTransaction();
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

        private IDictionary<int, Award> loadedAwardsList;
        protected virtual IDictionary<int, Award> LoadAwardsList()
        {
            if (loadedAwardsList == null)
            {
                loadedAwardsList = DatabaseSession.CreateCriteria<Award>().List<Award>().ToDictionary(x => x.AwardId);
            }
            return loadedAwardsList;
        }
    }
}