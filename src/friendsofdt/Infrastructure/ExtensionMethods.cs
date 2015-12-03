using System.Collections.Generic;
using System.Linq;

namespace FriendsOfDT {
    public static class ExtensionMethods {
        public static string BuildString(this IEnumerable<char> input) {
            return new string(input.ToArray());
        }

        public static string Take(this string input, int count) {
            return input.Take<char>(count).BuildString();
        }
    }
}