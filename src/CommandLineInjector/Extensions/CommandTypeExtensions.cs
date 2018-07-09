using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using CommandLineInjector.Exceptions;

namespace CommandLineInjector.Extensions
{
    public static class CommandTypeExtensions
    {
        /// <summary>
        /// Gets the description of this type, looking first for a <see cref="DescriptionAttribute"/> and falling back to just splitting up the name of the type
        /// </summary>
        /// <param name="type">A type, preferable with a <see cref="DescriptionAttribute"/> defined</param>
        /// <returns>A description of the type</returns>
        public static string GetDescription(this Type type)
        {
            return type.GetCustomAttribute<DescriptionAttribute>()?.Description ?? type.Name.SplitOutPascalCase();
        }
        
        /// <summary>
        /// Looks for the named method, falling back to the default Invoke() method if there is one on this command class
        /// </summary>
        /// <param name="commandType">A command class with an invoke method</param>
        /// <param name="methodName">Method name to search for</param>
        /// <returns>The method info of the most suitable invoke method</returns>
        public static MethodInfo FindMethodOrDefault(this Type commandType, string methodName = null)
        {
            var bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase;
            var fallbackBindingFlags = bindingFlags | BindingFlags.DeclaredOnly;
            var method = (methodName != null ? commandType.GetMethod(methodName, bindingFlags) : default(MethodInfo))
                ?? commandType.GetMethod("Invoke", bindingFlags)
                ?? commandType.GetMethod("Execute", bindingFlags)
                ?? commandType.GetMethods(fallbackBindingFlags).OrderByDescending(x => x.GetParameters().Length).FirstOrDefault();

            if (method == null)
                throw new InvalidCommandTypeException(commandType, "No suitable invoke method found");

            return method;
        }

    }
}
