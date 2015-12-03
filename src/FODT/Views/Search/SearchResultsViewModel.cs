using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FODT.Views.Search
{
    public class SearchResultsViewModel
    {
        public SearchResultsViewModel()
        {
            Results = new List<SearchResult>();
        }

        public string SearchTerm { get; set; }
        public List<SearchResult> Results { get; set; }

        public class SearchResult
        {
            public string Name { get; set; }
            public string Year { get; set; }
            public string ImageUrl { get; set; }
            public string LinkUrl { get; set; }
            public string SortField { get; set; }
        }

    }
}