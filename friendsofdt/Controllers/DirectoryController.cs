using System.Linq;
using System.Web.Mvc;
using FriendsOfDT.Models.Directory;
using RiaLibrary.Web;

namespace FriendsOfDT.Controllers {
    public partial class DirectoryController : AbstractController {
        public virtual ViewResult Index() {
            return View();
        }

        public virtual ViewResult ViewProfile() {
            return View();
        }

        [Authorize, AuthorizeRole()]
        [HttpGet, Url("Admin/DirectoryProfiles/List")]
        public virtual ViewResult AdminList() {
            return View();
        }

        [Authorize, AuthorizeRole()]
        [HttpGet, Url("Admin/DirectoryProfiles/New")]
        public virtual ViewResult AdminNew() {
            return View();
        }

        [Authorize, AuthorizeRole()]
        [HttpPost, Url("Admin/DirectoryProfiles/RegisterNewProfile")]
        public virtual RenderJsonResult RegisterNewProfile(RegisterNewProfileParameters parameters) {
            if (!parameters.AreValid()) {
                return this.RenderJsonErrorCode(1, "Invalid Parameters");
            }
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
            var newProfile = DirectoryProfile.RegisterNewProfile(parameters);
            DocumentSession.Store(newProfile);
            return this.RenderJsonSuccessErrorCode();
        }
    }
}
