using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AttributeRouting;
using AttributeRouting.Web.Mvc;
using FODT.Models.IMDT;
using FODT.Views.Search;
using NHibernate.Linq;

namespace FODT.Controllers
{
    [RoutePrefix("")]
    public partial class SearchController : BaseController
    {
        [GET("search")]
        public virtual ActionResult Search(string searchTerm, string searchType)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return RedirectToAction(MVC.Home.Welcome());
            }

            searchType = (searchType ?? "all").ToLower();

            var viewModel = new SearchResultsViewModel();
            viewModel.SearchTerm = searchTerm;

            var searchTerms = searchTerm.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);

            if (searchType == "all" || searchType == "show")
            {
                var query = DatabaseSession.Query<Show>();
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
                    ImageUrl = Url.Action(MVC.Media.GetItemTiny(x.MediaItem.MediaItemId)),
                    LinkUrl = Url.Action(MVC.Show.Get(x.ShowId)),
                }));
            }

            if (searchType == "all" || searchType == "peep")
            {
                var query = DatabaseSession.Query<Person>();
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
                    ImageUrl = Url.Action(MVC.Media.GetItemTiny(x.MediaItem.MediaItemId)),
                    LinkUrl = Url.Action(MVC.Person.Get(x.PersonId)),
                }));
            }

            if (viewModel.Results.Count == 1)
            {
                return Redirect(viewModel.Results[0].LinkUrl);
            }

            ViewBag.SearchTerm = searchTerm;
            return View("SearchResults", viewModel);
        }
    }
}
