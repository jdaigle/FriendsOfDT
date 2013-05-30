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
        public virtual ActionResult Search(string searchField, string searchType)
        {
            if (string.IsNullOrWhiteSpace(searchField))
            {
                return RedirectToAction(MVC.Home.Index());
            }

            searchType = searchType.ToLower() ?? "all";

            var viewModel = new SearchResultsViewModel();
            viewModel.SearchTerm = searchField;

            var searchTerms = searchField.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);

            if (searchType == "all" || searchType == "show")
            {
                var query = DatabaseSession.Query<Show>();
                foreach (var searchTerm in searchTerms)
                {
                    query = query.Where(x => x.Title.Contains(searchTerm));
                }
                var shows = query.ToList();
                viewModel.Results.AddRange(shows.Select(x => new SearchResultsViewModel.SearchResult
                {
                    Name = x.Title,
                    Year = x.Year.ToString(),
                    SortField = x.Title,
                    ImageUrl = Url.Action(MVC.Media.GetItemTiny(x.MediaItem.MediaItemId)),
                    LinkUrl = Url.Action(MVC.Shows.Display(x.ShowId)),
                }));
            }

            if (searchType == "all" || searchType == "peep")
            {
                var query = DatabaseSession.Query<Person>();
                foreach (var searchTerm in searchTerms)
                {
                    query = query.Where(x => x.LastName.Contains(searchTerm) ||
                                             x.FirstName.Contains(searchTerm) ||
                                             x.Nickname.Contains(searchTerm));
                }
                var people = query.ToList();
                viewModel.Results.AddRange(people.Select(x => new SearchResultsViewModel.SearchResult
                {
                    Name = x.Fullname,
                    SortField = x.LastName,
                    ImageUrl = Url.Action(MVC.Media.GetItemTiny(x.MediaItem.MediaItemId)),
                    LinkUrl = Url.Action(MVC.Person.Display(x.PersonId)),
                }));
            }

            if (viewModel.Results.Count == 1)
            {
                return Redirect(viewModel.Results[0].LinkUrl);
            }

            ViewBag.SearchTerm = searchField;
            return View("SearchResults", viewModel);
        }
    }
}
