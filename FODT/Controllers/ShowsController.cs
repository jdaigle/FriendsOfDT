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
                //.Include<Show>(x => x.Cast.Select(c => c.PersonId))
                //.Include<Show>(x => x.Crew.Select(c => c.PersonId))
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

            var viewModel = new DisplayViewModel();
            viewModel.ShowId = showId;
            viewModel.Name = show.Name;
            viewModel.Author = show.Author;
            viewModel.Quarter = show.Quarter;
            viewModel.Year = show.Year;

            //viewModel.Awards = show.Awards.Select(x => new DisplayViewModel.Award
            //{
            //    Year = x.Year,
            //    AwardId = x.AwardId,
            //    Name = this.LoadAwardsList()[x.AwardId],
            //    PersonId = DocumentSession.GetId<int?>(x.PersonId),
            //    PersonName = DocumentSession.GetId<int?>(x.PersonId).HasValue ? DocumentSession.Load<Person>(x.PersonId).Name : string.Empty,
            //});

            viewModel.Awards = awards.Select(x => new DisplayViewModel.Award
            {
                Year = x.AwardYear,
                AwardId = x.AwardId,
                Name = this.LoadAwardsList()[x.AwardId],
                PersonId = DocumentSession.GetId<int?>(x.PersonId),
                PersonName = x.PersonName,
            });

            viewModel.Cast = cast.Select(x => new DisplayViewModel.CastRole
            {
                PersonId = DocumentSession.GetId<int>(x.PersonId),
                //PersonName = DocumentSession.Load<Person>(x.PersonId).Name,
                PersonName = x.PersonName,
                Role = x.Role,
            }).ToList();

            viewModel.Crew = crew.Select(x => new DisplayViewModel.CrewPosition
            {
                PersonId = DocumentSession.GetId<int>(x.PersonId),
                //PersonName = DocumentSession.Load<Person>(x.PersonId).Name,
                PersonName = x.PersonName,
                CrewPositionId = x.CrewPositionId,
                Name = this.LoadCrewPositionsList()[x.CrewPositionId].Name,
            }).ToList();
            
            return View(viewModel);
        }
    }
}