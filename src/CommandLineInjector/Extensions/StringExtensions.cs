using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommandLineInjector.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// Creates a faux-description from this pascal case string by adding spaces before capital letters
        /// </summary>
        /// <param name="str">A pascal case string</param>
        /// <returns>The string with spaces added</returns>
        public static string SplitOutPascalCase(this string str)
        {
            return new string(str.ToCharArray().SplitOutPascalCase().ToArray());
        }

        /// <summary>
        /// Given a pascal case char array inserts spaces whenever there is a switch from a lowercase to uppercase character
        /// </summary>
        /// <param name="chars">Char array of a pascal case string</param>
        /// <returns></returns>
        public static IEnumerable<char> SplitOutPascalCase(this IEnumerable<char> chars)
        {
            char prev = default(char);
            foreach (var c in chars.Where(x => x != ' '))
            {
                if (prev != default(char) && char.IsUpper(c) && !char.IsUpper(prev))
                    yield return ' ';

                prev = c;
                yield return c;
            }
        }

        /// <summary>
        /// Returns the string with the first character in lowercase
        /// </summary>
        /// <param name="str">A string</param>
        public static string LowercaseFirstChar(this string str)
        {
            if (string.IsNullOrEmpty(str))
                return str;

            if (str.Length == 1)
                return str.ToLowerInvariant();

            return str.Substring(0, 1).ToLowerInvariant() + str.Substring(1);
        }
    }
}
