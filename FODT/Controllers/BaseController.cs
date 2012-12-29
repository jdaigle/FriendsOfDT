using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FODT.Models;
using FODT.Models.IMDT;
using Raven.Client;

namespace FODT.Controllers
{
    public abstract partial class BaseController : Controller
    {
        protected IDocumentSession DocumentSession;

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            this.DocumentSession = DocumentStoreConfiguration.DocumentStore.OpenSession();
            base.OnActionExecuting(filterContext);
        }

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (DocumentSession != null)
            {
                using (DocumentSession) { }
            }
            base.OnActionExecuted(filterContext);
        }

        private ClubPositionsList loadedClubPositionsList;
        protected virtual ClubPositionsList LoadClubPositionsList()
        {
            if (loadedClubPositionsList == null)
            {
                loadedClubPositionsList = DocumentSession.Load<ClubPositionsList>(ClubPositionsList.ID);
            }
            return loadedClubPositionsList;
        }

        private AwardsList loadedAwardsList;
        protected virtual AwardsList LoadAwardsList()
        {
            if (loadedAwardsList == null)
            {
                loadedAwardsList = DocumentSession.Load<AwardsList>(AwardsList.ID);
            }
            return loadedAwardsList;
        }

        private CrewPositionsList loadedCrewPositionsList;
        protected virtual CrewPositionsList LoadCrewPositionsList()
        {
            if (loadedCrewPositionsList == null)
            {
                loadedCrewPositionsList = DocumentSession.Load<CrewPositionsList>(CrewPositionsList.ID);
            }
            return loadedCrewPositionsList;
        }
    }
}