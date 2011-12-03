using System.Linq;
using System.Web.Mvc;
using FriendsOfDT.Models.Directory;
using Raven.Client.Linq;
using RiaLibrary.Web;

namespace FriendsOfDT.Controllers {
    [Authorize]
    public partial class DirectoryController : AbstractController {
        public virtual ViewResult Index() {
            return View();
        }

        public virtual ViewResult ViewProfile() {
            return View();
        }

        [AuthorizeRole()]
        [HttpGet, Url("Admin/DirectoryProfiles/List")]
        public virtual ViewResult AdminList() {
            return View();
        }

        [AuthorizeRole()]
        [HttpGet, Url("Admin/DirectoryProfiles/New")]
        public virtual ViewResult AdminNew() {
            return View();
        }

        [AjaxOnly]
        [AuthorizeRole()]
        public virtual RenderJsonResult ListProfiles(int? page, int? itemsPerPage) {
            page = page ?? 1;
            itemsPerPage = itemsPerPage ?? 20;
            RavenQueryStatistics stats = null;
            var results = DocumentSession.Query<DirectoryProfile>()
                .Statistics(out stats)
                //.OrderBy(x => x.LastName)
                .OrderBy(x => x.LastName)
                .Page(page.Value, itemsPerPage.Value)
                .ToList()
                .Select(x => new { id = x.Id, emailAddress = x.EmailAddress, lastName = x.LastName, firstName = x.FirstName, graduationYear = x.GraduationYear }).ToList();
            return new RenderJsonResult() { Data = new { items = results, count = stats.TotalResults } };
        }

        [AuthorizeRole()]
        [HttpPost, Url("Admin/DirectoryProfiles/RegisterNewProfile")]
        public virtual RenderJsonResult RegisterNewProfile(RegisterNewProfileParameters parameters, bool ignoreMatches) {
            if (!parameters.AreValid()) {
                return this.RenderJsonErrorCode(1, "Invalid Parameters");
            }
            if (!ignoreMatches) {
                var possibleMatchingProfiles = DocumentSession.Query<DirectoryProfile>()
                    .Where(x => x.FirstName.StartsWith(parameters.FirstName.Take(3)))
                    .Where(x => x.LastName.StartsWith(parameters.LastName.Take(3)))
                    .Take(5)
                    .ToList();
                if (possibleMatchingProfiles.Any()) {
                    return this.RenderJsonErrorCode(2, "Possible Existing Profiles", possibleMatchingProfiles.Select(x => new {
                        firstName = x.FirstName,
                        lastName = x.LastName,
                        emailAddress = x.EmailAddress,
                    }).ToArray());
                }
            }
            var newProfile = DirectoryProfile.RegisterNewProfile(parameters);
            DocumentSession.Store(newProfile);
            return this.RenderJsonSuccessErrorCode();
        }
    }
}
