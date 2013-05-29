using System;
using System.Linq;
using System.Web.Mvc;
using AttributeRouting;
using AttributeRouting.Web.Mvc;
using FODT.Database;
using FODT.Models;
using FODT.Models.Entities;
using FODT.Views.Shows;
using NHibernate.Linq;

namespace FODT.Controllers
{
    [RoutePrefix("Shows")]
    public partial class ShowsController : BaseController
    {
        [GET("{showId}")]
        public virtual ActionResult Display(int showId)
        {
            var show = DatabaseSession.Get<Show>(showId);

            var crew = DatabaseSession.Query<ShowCrew>().Where(x => x.Show == show).Fetch(x => x.Person).ToList();
            var cast = DatabaseSession.Query<ShowCast>().Where(x => x.Show == show).Fetch(x => x.Person).ToList();
            var awards = DatabaseSession.Query<ShowAward>().Where(x => x.Show == show).Fetch(x => x.Person).ToList();

            // TODO: replace with static index query
            var otherPerformances = DatabaseSession.Query<Show>()
                .Where(x => x.Title == show.Title)
                .ToList()
                .Where(x => x.ShowId != show.ShowId)
                .ToList();

            var viewModel = new DisplayViewModel();
            viewModel.ShowId = showId;
            viewModel.Name = show.Title;
            viewModel.Author = show.Author;
            viewModel.Quarter = (Quarter)show.Quarter;
            viewModel.Year = show.Year;

            viewModel.OtherPerformances = otherPerformances.Select(x => new System.Tuple<int, string, short>(x.ShowId, x.Title, x.Year)).ToList();

            viewModel.Awards = awards.Select(x => new DisplayViewModel.Award
            {
                Year = x.Year,
                AwardId = x.ShowAwardId,
                Name = this.LoadAwardsList()[x.Award.AwardId].Name,
                PersonId = x.Person != null ? x.Person.PersonId : (int?)null,
                PersonName = x.Person != null ? x.Person.LastName : (string)null,
                PersonLastName = x.Person != null ? x.Person.LastName : (string)null,
            }).ToList();

            viewModel.Cast = cast.Select(x => new DisplayViewModel.CastRole
            {
                PersonId = x.Person.PersonId,
                PersonName = x.Person.LastName,
                PersonLastName = x.Person.LastName,
                Role = x.Role,
            }).ToList();

            viewModel.Crew = crew.Select(x => new DisplayViewModel.CrewPosition
            {
                PersonId = x.Person.PersonId,
                PersonName = x.Person.LastName,
                PersonLastName = x.Person.LastName,
                Name = x.Position,
            }).ToList();

            return View(viewModel);
        }
    }
}