using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FODT.Models.IMDT;
using FODT.Security;
using FODT.Views.AwardsAdmin;
using Microsoft.Web.Mvc;
using NHibernate.Linq;

namespace FODT.Controllers
{
    [RoutePrefix("archive/awards")]
    [Authorize(Roles = RoleNames.Admin)]
    public class AwardsAdminController : BaseController
    {
        [HttpGet, Route("admin")]
        public ActionResult Admin()
        {
            var awardTypes = DatabaseSession.Query<AwardType>().ToList();


            return new ViewModelResult(new AwardTypesViewModel(awardTypes, this.Url));
        }


        [HttpGet]
        [AjaxOnly]
        [Route("types/{awardTypeId:int}/edit")]
        public ActionResult EditAwardType(int awardTypeId)
        {
            var awardType = DatabaseSession.Get<AwardType>(awardTypeId);
            var viewModel = new AwardTypeViewModel(awardType, this.Url);
            return PartialView(viewModel);
        }

        [HttpPost]
        [Route("types/{awardTypeId:int}/edit")]
        public ActionResult POSTEditEditAwardType(int awardTypeId, AwardTypeViewModel postModel)
        {
            var awardType = DatabaseSession.Get<AwardType>(awardTypeId);
            if (awardType == null)
            {
                return new HttpNotFoundResult();
            }

            if (string.IsNullOrWhiteSpace(postModel.Name))
            {
                return new HttpBadRequestResult("Name Must Not Be Null");
            }

            awardType.Name = postModel.Name;

            if (Request.IsAjaxRequest())
            {
                return Json("OK");
            }

            return this.RedirectToAction(c => c.Admin());
        }

        [HttpPost]
        [Route("types/add")]
        public ActionResult POSTAddEditAwardType(AwardTypeViewModel postModel)
        {
            if (string.IsNullOrWhiteSpace(postModel.Name))
            {
                return new HttpBadRequestResult("Name Must Not Be Null");
            }

            var awardType = new AwardType();
            awardType.Name = postModel.Name;
            DatabaseSession.Save(awardType);

            if (Request.IsAjaxRequest())
            {
                return Json("OK");
            }

            return this.RedirectToAction(c => c.Admin());
        }
    }
}