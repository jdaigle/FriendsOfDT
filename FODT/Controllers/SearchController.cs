using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AttributeRouting;
using AttributeRouting.Web.Mvc;
using FODT.Models.Entities;
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

            if (searchType == "all" || searchType == "show")
            {
                var shows = DatabaseSession.Query<Show>().Where(x => x.Title.Contains(searchField)).ToList();
                viewModel.Results.AddRange(shows.Select(x => new SearchResultsViewModel.SearchResult
                {
                    Name = x.Title,
                    Year = x.Year.ToString(),
                    SortField = x.Title,
                    ImageUrl = Url.Content("~/content/nopicind.gif"),
                    LinkUrl = Url.Action(MVC.Shows.Display(x.ShowId)),
                }));
            }

            if (searchType == "all" || searchType == "peep")
            {
                var people = DatabaseSession.Query<Person>().Where(x => x.LastName.Contains(searchField) ||
                                                                        x.FirstName.Contains(searchField) ||
                                                                        x.Nickname.Contains(searchField)).ToList();
                viewModel.Results.AddRange(people.Select(x => new SearchResultsViewModel.SearchResult
                {
                    Name = x.Fullname,
                    SortField = x.LastName,
                    ImageUrl = Url.Content("~/content/nopicind.gif"),
                    LinkUrl = Url.Action(MVC.Person.Display(x.PersonId)),
                }));
            }

            ViewBag.SearchTerm = searchField;
            return View("SearchResults", viewModel);
        }
    }
}
