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
        public virtual ActionResult Get(int personId)
        {
            var person = DatabaseSession.Get<Person>(personId);
            var clubPositions = DatabaseSession.Query<PersonClubPosition>().Where(x => x.Person == person).ToList();
            var crew = DatabaseSession.Query<ShowCrew>().Where(x => x.Person == person).Fetch(x => x.Show).ToList();
            var cast = DatabaseSession.Query<ShowCast>().Where(x => x.Person == person).Fetch(x => x.Show).ToList();
            var showAwards = DatabaseSession.Query<ShowAward>().Where(x => x.Person == person).Fetch(x => x.Show).Fetch(x => x.Award).ToList();
            var myAwards = DatabaseSession.Query<PersonAward>().Where(x => x.Person == person).Fetch(x => x.Award).ToList();

            var viewModel = new GetViewModel();
            viewModel.PersonId = personId;
            viewModel.FullName = person.Fullname;
            viewModel.Biography = person.Biography;
            viewModel.MediaItemId = person.MediaItem.MediaItemId;
            viewModel.ClubPositions = clubPositions.Select(x => new GetViewModel.ClubPosition
            {
                Year = x.Year,
                Name = x.Position,
            }).ToList();
            viewModel.Awards = showAwards.Select(x => new GetViewModel.Award
            {
                Year = x.Year,
                AwardId = x.ShowAwardId,
                Name = x.Award.Name,
                ShowId = x.Show.ShowId,
                ShowName = x.Show.Title,
                ShowQuarter = (Quarter)x.Show.Quarter,
                ShowYear = x.Show.Year,
            })
            .Concat(myAwards.Select(x => new GetViewModel.Award
            {
                Year = x.Year,
                AwardId = x.PersonAwardId,
                Name = x.Award.Name,
            })).ToList();
            viewModel.CastRoles = cast.Select(x => new GetViewModel.CastRole
            {
                ShowId = x.Show.ShowId,
                ShowName = x.Show.Title,
                ShowQuarter = (Quarter)x.Show.Quarter,
                ShowYear = x.Show.Year,
                Role = x.Role,
            }).ToList();
            viewModel.CrewPositions = crew.Select(x => new GetViewModel.CrewPosition
            {
                ShowId = x.Show.ShowId,
                ShowName = x.Show.Title,
                ShowQuarter = (Quarter)x.Show.Quarter,
                ShowYear = x.Show.Year,
                Name = x.Position,
            }).ToList();
            return View(viewModel);
        }

        [GET("{personId}/Media")]
        public virtual ActionResult ListMedia(int personId)
        {
            throw new NotImplementedException();
        }

        [GET("{personId}/Media/{mediaId}")]
        public virtual ActionResult GetMedia(int personId, int mediaId)
        {
            throw new NotImplementedException();
        }

        [GET("{personId}/Edit")]
        public virtual ActionResult Edit(int personId)
        {
            var person = DatabaseSession.Get<Person>(personId);

            var viewModel = new EditViewModel();
            viewModel.PersonId = personId;
            viewModel.Name = person.FirstName;
            viewModel.Biography = person.Biography;
            return View(viewModel);
        }

        [POST("{personId}/Edit")]
        public virtual ActionResult SaveEdit(int personId, SaveEditParameters param)
        {
            var person = DatabaseSession.Get<Person>(personId);
            if (string.IsNullOrWhiteSpace(param.Name))
            {
                throw new Exception("Name is required");
            }
            //person.Name = model.Name.Trim();
            person.Biography = (param.Biography ?? string.Empty).Trim();

            DatabaseSession.CommitTransaction();
            return RedirectToAction(Actions.Get(personId));
        }

        public class SaveEditParameters
        {
            [Required]
            public string Name { get; set; }
            [AllowHtml]
            public string Biography { get; set; }
        }
    }
}