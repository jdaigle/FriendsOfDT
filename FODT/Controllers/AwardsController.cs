using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AttributeRouting;
using AttributeRouting.Web.Mvc;
using NHibernate.Linq;
using FODT.Models.IMDT;

namespace FODT.Controllers
{
    [RoutePrefix("awards")]
    public partial class AwardsController : BaseController
    {
        [GET("year")]
        public virtual ActionResult ByYear()
        {
            var mostRecentShowYear = DatabaseSession.Query<ShowAward>().OrderByDescending(x => x.Year).Take(1).Select(x => x.Year).Single();
            var mostRecentPersonYear = DatabaseSession.Query<PersonAward>().OrderByDescending(x => x.Year).Take(1).Select(x => x.Year).Single();
            var mostRecentYear = mostRecentShowYear > mostRecentPersonYear ? mostRecentShowYear : mostRecentPersonYear;
            return RedirectToAction(Actions.ByYear(mostRecentYear));
        }

        [GET("year/{year}")]
        public virtual ActionResult ByYear(short year)
        {
            var showAwards = DatabaseSession.Query<ShowAward>().Where(x => x.Year == year).Fetch(x => x.Show).ToList();
            var peopleAwards = DatabaseSession.Query<PersonAward>().Where(x => x.Year == year).Fetch(x => x.Person).ToList();
            throw new NotImplementedException();
        }
    }
}