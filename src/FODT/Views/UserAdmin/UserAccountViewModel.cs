using FODT.Controllers;
using FODT.Models.FODT;
using System.Globalization;
using System.Web.Mvc;

namespace FODT.Views.UserAdmin
{
    public class UserAccountViewModel
    {
        public UserAccountViewModel() { }

        public UserAccountViewModel(UserAccount user, UrlHelper url)
        {
            Name = user.Name;
            Email = user.Email;
            CreatedDateTime = user.InsertedDateTime.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            LastSeenDateTime = user.LastSeenDateTime?.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);
            IsContributor = user.IsContributor;
            IsArchivist = user.IsArchivist;
            IsAdmin = user.IsAdmin;
            EditURL = url.GetURL<UserAdminController>(c => c.EditUser(user.UserAccountId));
        }

        public string Name { get; set; }
        public string Email { get; set; }
        public string CreatedDateTime { get; set; }
        public string LastSeenDateTime { get; set; }
        public bool IsContributor { get; set; }
        public bool IsArchivist { get; set; }
        public bool IsAdmin { get; set; }
        public string EditURL { get; set; }
    }
}