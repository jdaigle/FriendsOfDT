using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FODT.Models.IMDT
{
    public static class ShowComparer
    {
        public static int ChronologicalShowComparison(Show x, Show y)
        {
            if (object.ReferenceEquals(x, y))
            {
                return 0;
            }

            if (x.Year < y.Year)
            {
                return -1;
            }
            if (x.Year > y.Year)
            {
                return 1;
            }

            if ((byte)x.Quarter < (byte)y.Quarter)
            {
                return -1;
            }
            if ((byte)x.Quarter < (byte)y.Quarter)
            {
                return 1;
            }

            return string.Compare(x.Title, y.Title, true);
        }

        public static int ReverseChronologicalShowComparison(Show x, Show y)
        {
            if (object.ReferenceEquals(x, y))
            {
                return 0;
            }

            if (x.Year < y.Year)
            {
                return 1;
            }
            if (x.Year > y.Year)
            {
                return -1;
            }

            if ((byte)x.Quarter < (byte)y.Quarter)
            {
                return 1;
            }
            if ((byte)x.Quarter < (byte)y.Quarter)
            {
                return -1;
            }

            return string.Compare(x.Title, y.Title, true);
        }

        public static readonly Comparer<Show> ChronologicalShowComparer = Comparer<Show>.Create(ChronologicalShowComparison);
        public static readonly Comparer<Show> ReverseChronologicalShowComparer = Comparer<Show>.Create(ReverseChronologicalShowComparison);
    }
}