using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace FODT.Models
{
    public static class ExtensionMethods
    {
        /// <summary>
        /// Rearranges a show title so that titles of the format "Foo, The", "Foo, A" (name comma something)
        /// display correctly (sorted without the "The" or "A"
        /// </summary>
        public static string RearrangeShowTitle(string title)
        {
            if (Regex.Match(title, ", The$").Success)
            {
                return "The " + title.Substring(0, title.Length - 5);
            }

            if (Regex.Match(title, ", A$").Success)
            {
                return "A " + title.Substring(0, title.Length - 3);
            }

            if (Regex.Match(title, ", An$").Success)
            {
                return "An " + title.Substring(0, title.Length - 4);
            }

            return title;
        }
    }
}