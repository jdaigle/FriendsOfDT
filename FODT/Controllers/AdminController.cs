using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FODT.Models.FODT;
using FODT.Models.IMDT;
using NHibernate.Linq;

namespace FODT.Controllers
{
    [RoutePrefix("admin")]
    public partial class AdminController : BaseController
    {
        [HttpGet, Route("")]
        public virtual ActionResult Index()
        {
            return View();
        }

        [HttpGet, Route("users/list")]
        public virtual ActionResult UsersList()
        {
            var users = this.DatabaseSession.Query<UserAccount>().ToList();
            var viewModel = users
                .Select(x => new
                {
                    Id = x.UserAccountId,
                    Name = x.Name,
                    Email = x.Email,
                    IsFacebook = !string.IsNullOrWhiteSpace(x.FacebookId),
                    FacebookPictureURL = x.FacebookPictureURL,
                    FacebookURL = x.FacebookURL,
                    LastLoginDateTime = x.LastLoginDateTime.ToString("o"),
                }.ToExpando() as dynamic)
                .OrderBy(x => x.Name)
                .ToList();
            return View(viewModel);
        }

        [HttpGet, Route("people/list")]
        public virtual ActionResult PeopleList()
        {
            var users = this.DatabaseSession.Query<Person>().ToList();
            var viewModel = users
                .Select(x => new
                {
                    Id = x.PersonId,
                    Name = x.Fullname,
                    SortName = x.SortableName
                }.ToExpando() as dynamic)
                .OrderBy(x => x.SortName)
                .ToList();
            return View(viewModel);
        }
    }
}