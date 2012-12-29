using System.Linq;
using System.Web.Mvc;
using AttributeRouting;
using AttributeRouting.Web.Mvc;
using FODT.Models;
using FODT.Models.IMDT;
using FODT.Models.IMDT.Indexes;
using FODT.Views.People;
using Raven.Client;
using Raven.Client.Linq;

namespace FODT.Controllers
{
    [RoutePrefix("People")]
    public partial class PeopleController : BaseController
    {
        [GET("{personId}")]
        public virtual ActionResult Display(int personId)
        {
            var person = DocumentSession.Load<Person>(personId);
            var crew = DocumentSession.Query<CrewProjection, Shows_Crew>()
                .Where(x => x.PersonId == ("people/" + person.Id).ToString()).AsProjection<CrewProjection>().ToList();
            var cast = DocumentSession.Query<CastProjection, Shows_Cast>()
                .Where(x => x.PersonId == ("people/" + person.Id).ToString()).AsProjection<CastProjection>().ToList();
            var showAwards = DocumentSession.Query<AwardProjection, Awards>()
                .Where(x => x.PersonId == ("people/" + person.Id).ToString()).AsProjection<AwardProjection>().ToList();
            
            var viewModel = new DisplayViewModel();
            viewModel.PersonId = personId;
            viewModel.Name = person.Name;
            viewModel.Biography = person.Biography;
            viewModel.ClubPositions = person.ClubPositions.Select(x => new DisplayViewModel.ClubPosition
            {
                Year = x.Year,
                ClubPositionId = x.ClubPositionId,
                Name = this.LoadClubPositionsList()[x.ClubPositionId],
            }).ToList();
            viewModel.Awards = showAwards.Select(x => new DisplayViewModel.Award
            {
                Year = x.AwardYear,
                AwardId = x.AwardId,
                Name = this.LoadAwardsList()[x.AwardId],
                ShowId = DocumentSession.GetId<int?>(x.GetShowId()),
                ShowName = x.ShowName,
                ShowYear = x.ShowYear,
            }).ToList();
            viewModel.CastRoles = cast.Select(x => new DisplayViewModel.CastRole
            {
                ShowId = DocumentSession.GetId<int>(x.__document_id),
                ShowName = x.ShowName,
                ShowQuarter = x.ShowQuarter,
                ShowYear = x.ShowYear,
                Role = x.Role,
            }).ToList();
            viewModel.CrewPositions = crew.Select(x => new DisplayViewModel.CrewPosition
            {
                ShowId = DocumentSession.GetId<int>(x.__document_id),
                ShowName = x.ShowName,
                ShowQuarter = x.ShowQuarter,
                ShowYear = x.ShowYear,
                Name = this.LoadCrewPositionsList()[x.CrewPositionId].Name,
                CrewPositionId = x.CrewPositionId,
            }).ToList();
            return View(viewModel);
        }
    }
}