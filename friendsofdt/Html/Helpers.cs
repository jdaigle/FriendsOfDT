using System.Collections.Generic;
using System.Dynamic;
using System.Web.Mvc;

namespace FriendsOfDT.Html {
    public static class Helpers {
        public static ExpandoObject ToExpando(this object anonymousObject) {
            var anonymousDictionary = HtmlHelper.AnonymousObjectToHtmlAttributes(anonymousObject);
            IDictionary<string, object> expando = new ExpandoObject();
            foreach (var item in anonymousDictionary)
                expando.Add(item);
            return (ExpandoObject)expando;
        }
    }
}