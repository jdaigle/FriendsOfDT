using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FODT.Models.IMDT;
using FODT.Views.Search;
using NHibernate.Linq;

namespace FODT.Controllers
{
    [RoutePrefix("archive")]
    public class SearchController : BaseController
    {
        [HttpGet, Route("search")]
        public ActionResult Search(string searchTerm, string searchType)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return Redirect("~");
            }

            searchType = (searchType ?? "all").ToLower();

            var viewModel = new SearchResultsViewModel();
            viewModel.SearchTerm = searchTerm;

            var searchTerms = searchTerm.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);

            if (searchType == "all" || searchType == "show")
            {
                var query = DatabaseSession.Query<Show>()
                    .Fetch(x => x.Photo).AsQueryable();
                foreach (var term in searchTerms)
                {
                    query = query.Where(x => x.Title.Contains(term));
                }
                var shows = query.ToList();
                viewModel.Results.AddRange(shows.Select(x => new SearchResultsViewModel.SearchResult
                {
                    Name = x.Title,
                    Year = x.Year.ToString(),
                    SortField = x.Title,
                    ImageUrl = x.Photo.GetTinyFileURL(),
                    LinkUrl = Url.Action<ShowController>(c => c.ShowDetails(x.ShowId)),
                }));
            }

            if (searchType == "all" || searchType == "peep")
            {
                var query = DatabaseSession.Query<Person>()
                    .Fetch(x => x.Photo).AsQueryable();
                foreach (var term in searchTerms)
                {
                    query = query.Where(x => x.LastName.Contains(term) ||
                                             x.FirstName.Contains(term) ||
                                             x.Nickname.Contains(term));
                }
                var people = query.ToList();
                viewModel.Results.AddRange(people.Select(x => new SearchResultsViewModel.SearchResult
                {
                    Name = x.Fullname,
                    SortField = x.LastName,
                    ImageUrl = x.Photo.GetTinyFileURL(),
                    LinkUrl = Url.Action<PersonController>(c => c.PersonDetails(x.PersonId)),
                }));
            }

            if (viewModel.Results.Count == 1)
            {
                return Redirect(viewModel.Results[0].LinkUrl);
            }

            ViewBag.SearchTerm = searchTerm;
            return View(viewModel);
        }
    }
}
