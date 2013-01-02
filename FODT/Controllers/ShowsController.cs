using System;
using System.Linq;
using System.Web.Mvc;
using AttributeRouting;
using AttributeRouting.Web.Mvc;
using FODT.Models;
using FODT.Models.IMDT;
using FODT.Models.IMDT.Indexes;
using FODT.Views.Shows;
using Raven.Client;
using Raven.Client.Linq;

namespace FODT.Controllers
{
    [RoutePrefix("Shows")]
    public partial class ShowsController : BaseController
    {
        [GET("{showId}")]
        public virtual ActionResult Display(int showId)
        {
            var show = DocumentSession
                .Load<Show>(showId);

            var awards = DocumentSession.Query<AwardProjection, Awards>()
                .Where(x => x.__document_id == ("shows/" + showId).ToString())
                .AsProjection<AwardProjection>().ToList();

            var cast = DocumentSession.Query<CastProjection, Shows_Cast>()
                .Where(x => x.__document_id == ("shows/" + showId).ToString())
                .AsProjection<CastProjection>().ToList();

            var crew = DocumentSession.Query<CrewProjection, Shows_Crew>()
                .Where(x => x.__document_id == ("shows/" + showId).ToString())
                .AsProjection<CrewProjection>().ToList();

            // TODO: replace with static index query
            var otherPerformances = DocumentSession.Query<Show>()
                .Where(x => x.Name == show.Name)
                .ToList()
                .Where(x => x.Id != show.Id)
                .ToList();

            var viewModel = new DisplayViewModel();
            viewModel.ShowId = showId;
            viewModel.Name = show.Name;
            viewModel.Author = show.Author;
            viewModel.Quarter = show.Quarter;
            viewModel.Year = show.Year;

            viewModel.OtherPerformances = otherPerformances.Select(x => new Tuple<int, string, short>(x.Id, x.Name, x.Year)).ToList();

            viewModel.Awards = awards.Select(x => new DisplayViewModel.Award
            {
                Year = x.AwardYear,
                AwardId = x.AwardId,
                Name = this.LoadAwardsList()[x.AwardId],
                PersonId = DocumentSession.GetId<int?>(x.PersonId),
                PersonName = x.PersonFullName,
                PersonLastName = x.PersonLastName,
            });

            viewModel.Cast = cast.Select(x => new DisplayViewModel.CastRole
            {
                PersonId = DocumentSession.GetId<int>(x.PersonId),
                PersonName = x.PersonFullName,
                PersonLastName = x.PersonLastName,
                Role = x.Role,
            }).ToList();

            viewModel.Crew = crew.Select(x => new DisplayViewModel.CrewPosition
            {
                PersonId = DocumentSession.GetId<int>(x.PersonId),
                PersonName = x.PersonFullName,
                PersonLastName = x.PersonLastName,
                CrewPositionId = x.CrewPositionId,
                Name = this.LoadCrewPositionsList()[x.CrewPositionId].Name,
            }).ToList();
            
            return View(viewModel);
        }
    }
}