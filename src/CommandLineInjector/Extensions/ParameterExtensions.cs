using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Text;

namespace CommandLineInjector.Extensions
{
    public static class ParameterExtensions
    {
        /// <summary>
        /// Indexes the parameters by creating shortcut names for each parameter and returning the result in a dictionary
        /// </summary>
        /// <param name="parameterInfos">Set of method parameters with unique names</param>
        /// <param name="excludeKeys">Shortcut keys to exclude</param>s
        /// <returns>The parameters indexed by their new shortcut names</returns>
        public static IDictionary<string, ParameterInfo> IndexShortenedNames(this IEnumerable<ParameterInfo> parameterInfos, HashSet<string> excludeKeys)
        {
            var dictionary = new Dictionary<string, ParameterInfo>();
            foreach (var parameterInfo in parameterInfos)
            {
                string MakeKey(int length) => parameterInfo.Name.Substring(0, length).ToLower();

                if (parameterInfo.HasDefaultValue)
                {
                    int length = 1;
                    while (dictionary.ContainsKey(MakeKey(length)) || excludeKeys.Contains(MakeKey(length)))
                        length++;
                    dictionary.Add(MakeKey(length), parameterInfo);
                }
                else
                {
                    dictionary.Add(parameterInfo.Name, parameterInfo);
                }
            }
            return dictionary;
        }

        /// <summary>
        /// Gets the description of this parameter, looking first for a <see cref="DescriptionAttribute"/> and falling back to just splitting up the name of the parameter
        /// </summary>
        /// <param name="parameterInfo">A parameter, preferable with a <see cref="DescriptionAttribute"/> defined</param>
        /// <returns>A description of the type</returns>
        public static string GetHelpText(this ParameterInfo parameterInfo)
        {
            return parameterInfo.GetCustomAttribute<DescriptionAttribute>()?.Description ?? parameterInfo.Name.SplitOutPascalCase();
        }

        /// <summary>
        /// Returns true if this parameter requires a value to be passed in the command line
        /// </summary>
        /// <param name="parameterInfo">A parameter</param>
        /// <returns>True if this parameter needs an accompanying value</returns>
        public static bool RequiresPassedValue(this ParameterInfo parameterInfo)
        {
            return parameterInfo.ParameterType != typeof(bool) && parameterInfo.ParameterType != typeof(bool?);
        }

    }
}
