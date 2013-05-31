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
using FODT.Views.Shared;
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
                ShowQuarter = x.Show.Quarter,
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
                ShowQuarter = x.Show.Quarter,
                ShowYear = x.Show.Year,
                Role = x.Role,
            }).ToList();
            viewModel.CrewPositions = crew.Select(x => new GetViewModel.CrewPosition
            {
                ShowId = x.Show.ShowId,
                ShowName = x.Show.Title,
                ShowQuarter = x.Show.Quarter,
                ShowYear = x.Show.Year,
                Name = x.Position,
            }).ToList();
            return View(viewModel);
        }

        [GET("{personId}/Media")]
        public virtual ActionResult ListPersonMedia(int personId)
        {
            var person = DatabaseSession.Get<Person>(personId);
            var relatedMedia = DatabaseSession.Query<PersonMedia>().Where(x => x.Person == person).Fetch(x => x.MediaItem).ToList();

            var viewModel = new ListPersonMediaViewModel();
            viewModel.PersonId = personId;
            viewModel.PersonFullname = person.Fullname;
            viewModel.RelatedMedia = relatedMedia.OrderBy(x => x.MediaItem.InsertedDateTime).ThenBy(x => x.MediaItem.MediaItemId).Select(x => new ListPersonMediaViewModel.Media
            {
                MediaItemId = x.MediaItem.MediaItemId,
            }).ToList();
            return View(viewModel);
        }

        [GET("{personId}/Media/{mediaItemId}")]
        public virtual ActionResult GetPersonMedia(int personId, int mediaItemId)
        {
            var person = DatabaseSession.Get<Person>(personId);
            var relatedMedia = DatabaseSession
                .Query<PersonMedia>().Where(x => x.Person == person).Fetch(x => x.MediaItem)
                .ToList()
                .OrderBy(x => x.MediaItem.InsertedDateTime).ThenBy(x => x.MediaItem.MediaItemId)
                .ToList();
            var media = relatedMedia.Single(x => x.MediaItem.MediaItemId == mediaItemId);

            var index = relatedMedia.IndexOf(relatedMedia.Single(x => x.PersonMediaId == media.PersonMediaId));
            var previousId = index > 0 ? relatedMedia[index - 1].MediaItem.MediaItemId : (int?)null;
            var nextId = index < relatedMedia.Count - 1 ? relatedMedia[index + 1].MediaItem.MediaItemId : (int?)null;

            var relatedPeople = DatabaseSession.Query<PersonMedia>().Where(x => x.MediaItem == media.MediaItem).Fetch(x => x.Person).ToList();
            var relatedShows = DatabaseSession.Query<ShowMedia>().Where(x => x.MediaItem == media.MediaItem).Fetch(x => x.Show).ToList();

            var viewModel = new GetPersonMediaViewModel();
            viewModel.PersonId = personId;
            viewModel.PersonFullname = person.Fullname;
            viewModel.PreviousId = previousId;
            viewModel.NextId = nextId;
            viewModel.MediaItemId = media.MediaItem.MediaItemId;
            viewModel.MediaItemViewModel = new MediaItemViewModel();
            viewModel.MediaItemViewModel.Id = media.MediaItem.MediaItemId;
            viewModel.MediaItemViewModel.RelatedShows = relatedShows.Select(x => new MediaItemViewModel.RelatedShow
            {
                ShowId = x.Show.ShowId,
                ShowQuarter = x.Show.Quarter,
                ShowYear = x.Show.Year,
                ShowTitle = x.Show.Title,
            }).ToList();
            viewModel.MediaItemViewModel.RelatedPeople = relatedPeople.Select(x => new MediaItemViewModel.RelatedPerson
            {
                PersonId = x.Person.PersonId,
                PersonLastName = x.Person.LastName,
                PersonFullname = x.Person.Fullname,
            }).ToList();
            return View(viewModel);
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