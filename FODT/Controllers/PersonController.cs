using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using AttributeRouting;
using AttributeRouting.Web.Mvc;
using FODT.Database;
using FODT.Models;
using FODT.Models.IMDT;
using FODT.Views.Person;
using NHibernate.Linq;

namespace FODT.Controllers
{
    [RoutePrefix("Person")]
    public partial class PersonController : BaseController
    {
        [GET("{personId}")]
        public virtual ActionResult Display(int personId)
        {
            var person = DatabaseSession.Get<Person>(personId);
            var clubPositions = DatabaseSession.Query<PersonClubPosition>().Where(x => x.Person == person).ToList();
            var crew = DatabaseSession.Query<ShowCrew>().Where(x => x.Person == person).Fetch(x => x.Show).ToList();
            var cast = DatabaseSession.Query<ShowCast>().Where(x => x.Person == person).Fetch(x => x.Show).ToList();
            var showAwards = DatabaseSession.Query<ShowAward>().Where(x => x.Person == person).Fetch(x => x.Show).Fetch(x => x.Award).ToList();
            var myAwards = DatabaseSession.Query<PersonAward>().Where(x => x.Person == person).Fetch(x => x.Award).ToList();

            var viewModel = new DisplayViewModel();
            viewModel.PersonId = personId;
            viewModel.FullName = person.Fullname;
            viewModel.Biography = person.Biography;
            viewModel.MediaItemId = person.MediaItem.MediaItemId;
            viewModel.ClubPositions = clubPositions.Select(x => new DisplayViewModel.ClubPosition
            {
                Year = x.Year,
                Name = x.Position,
            }).ToList();
            viewModel.Awards = showAwards.Select(x => new DisplayViewModel.Award
            {
                Year = x.Year,
                AwardId = x.ShowAwardId,
                Name = x.Award.Name,
                ShowId = x.Show.ShowId,
                ShowName = x.Show.Title,
                ShowQuarter = (Quarter)x.Show.Quarter,
                ShowYear = x.Show.Year,
            })
            .Concat(myAwards.Select(x => new DisplayViewModel.Award
            {
                Year = x.Year,
                AwardId = x.PersonAwardId,
                Name = x.Award.Name,
            })).ToList();
            viewModel.CastRoles = cast.Select(x => new DisplayViewModel.CastRole
            {
                ShowId = x.Show.ShowId,
                ShowName = x.Show.Title,
                ShowQuarter = (Quarter)x.Show.Quarter,
                ShowYear = x.Show.Year,
                Role = x.Role,
            }).ToList();
            viewModel.CrewPositions = crew.Select(x => new DisplayViewModel.CrewPosition
            {
                ShowId = x.Show.ShowId,
                ShowName = x.Show.Title,
                ShowQuarter = (Quarter)x.Show.Quarter,
                ShowYear = x.Show.Year,
                Name = x.Position,
            }).ToList();
            return View(viewModel);
        }


        [GET("{personId}/Edit")]
        public virtual ActionResult EditBiography(int personId)
        {
            var person = DatabaseSession.Get<Person>(personId);

            var viewModel = new EditBiographyViewModel();
            viewModel.PersonId = personId;
            viewModel.Name = person.FirstName;
            viewModel.Biography = person.Biography;
            return View(viewModel);
        }

        [POST("{personId}/Edit")]
        public virtual ActionResult SaveEditBiography(int personId, SaveEditBiographyModel model)
        {
            var person = DatabaseSession.Get<Person>(personId);
            if (string.IsNullOrWhiteSpace(model.Name))
            {
                throw new Exception("Name is required");
            }
            //person.Name = model.Name.Trim();
            person.Biography = (model.Biography ?? string.Empty).Trim();

            DatabaseSession.CommitTransaction();
            return RedirectToAction(Actions.Display(personId));
        }

        public class SaveEditBiographyModel
        {
            [Required]
            public string Name { get; set; }
            [AllowHtml]
            public string Biography { get; set; }
        }
    }
}