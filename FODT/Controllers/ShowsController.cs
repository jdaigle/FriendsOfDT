using System;
using System.Linq;
using System.Web.Mvc;
using AttributeRouting;
using AttributeRouting.Web.Mvc;
using FODT.Database;
using FODT.Models;
using FODT.Models.IMDT;
using FODT.Views.Shows;
using NHibernate.Linq;

namespace FODT.Controllers
{
    [RoutePrefix("Shows")]
    public partial class ShowsController : BaseController
    {
        public class ShowOrderDto
        {
            public virtual int ShowId { get; set; }
            public virtual string Title { get; set; }
            public virtual byte Quarter { get; set; }
            public virtual short Year { get; set; }
        }

        [GET("{showId}")]
        public virtual ActionResult Get(int showId)
        {
            var orderedShows = DatabaseSession.Query<Show>().Select(x => new ShowOrderDto()
                {
                    ShowId = x.ShowId,
                    Quarter = x.Quarter,
                    Title = x.Title,
                    Year = x.Year,
                }).ToList().OrderBy(x => x.Year).ThenBy(x => x.Quarter).ThenBy(x => x.Title).ToList();
            var index = orderedShows.IndexOf(orderedShows.Single(x => x.ShowId == showId));
            var previousShowId = index > 0 ? orderedShows[index - 1].ShowId : (int?)null;
            var nextShowId = index < orderedShows.Count - 1 ? orderedShows[index + 1].ShowId : (int?)null;

            var show = DatabaseSession.Get<Show>(showId);

            var crew = DatabaseSession.Query<ShowCrew>().Where(x => x.Show == show).Fetch(x => x.Person).ToList();
            var cast = DatabaseSession.Query<ShowCast>().Where(x => x.Show == show).Fetch(x => x.Person).ToList();
            var awards = DatabaseSession.Query<ShowAward>().Where(x => x.Show == show).Fetch(x => x.Person).Fetch(x => x.Award).ToList();

            var otherPerformances = DatabaseSession.Query<Show>()
                .Where(x => x.Title == show.Title && 
                            x.ShowId != show.ShowId)
                .ToList();

            var viewModel = new GetViewModel();
            viewModel.ShowId = showId;
            viewModel.Title = show.Title;
            viewModel.Author = show.Author;
            viewModel.Quarter = (Quarter)show.Quarter;
            viewModel.Year = show.Year;
            viewModel.FunFacts = show.FunFacts;
            viewModel.Pictures = show.Pictures;
            viewModel.Toaster = show.Toaster;
            viewModel.MediaItemId = show.MediaItem.MediaItemId;
            viewModel.PreviousShowId = previousShowId;
            viewModel.NextShowId = nextShowId;

            viewModel.OtherPerformances = otherPerformances.Select(x => new System.Tuple<int, string, short>(x.ShowId, x.Title, x.Year)).ToList();

            viewModel.Awards = awards.Select(x => new GetViewModel.Award
            {
                Year = x.Year,
                AwardId = x.ShowAwardId,
                Name = x.Award.Name,
                PersonId = x.Person != null ? x.Person.PersonId : (int?)null,
                PersonName = x.Person != null ? x.Person.Fullname : (string)null,
                PersonLastName = x.Person != null ? x.Person.LastName : (string)null,
            }).ToList();

            viewModel.Cast = cast.Select(x => new GetViewModel.CastRole
            {
                PersonId = x.Person.PersonId,
                PersonName = x.Person.Fullname,
                PersonLastName = x.Person.LastName,
                Role = x.Role,
            }).ToList();

            viewModel.Crew = crew.Select(x => new GetViewModel.CrewPosition
            {
                PersonId = x.Person.PersonId,
                PersonName = x.Person.Fullname,
                PersonLastName = x.Person.LastName,
                Name = x.Position,
                DisplayOrder = x.DisplayOrder,
            }).ToList();

            return View(viewModel);
        }

        [GET("{showId}/Media")]
        public virtual ActionResult ListMedia(int showId)
        {
            throw new NotImplementedException();
        }

        [GET("{showId}/Media/{mediaId}")]
        public virtual ActionResult GetMedia(int showId, int mediaId)
        {
            throw new NotImplementedException();
        }
    }
}