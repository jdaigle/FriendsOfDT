using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AttributeRouting;
using AttributeRouting.Web.Mvc;

namespace FODT.Controllers
{
    [RoutePrefix("awards")]
    public partial class AwardsController : BaseController
    {
        [GET("year/{year}")]
        public virtual ActionResult ByYear(short year)
        {
            throw new NotImplementedException();
        }
    }
}