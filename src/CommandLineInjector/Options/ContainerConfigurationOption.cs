using System;
using System.Collections.Generic;
using System.Text;

namespace CommandLineInjector.Options
{
    /// <summary>
    /// Defines a command line option that is used to configure a child IOC container before resolving the services
    /// </summary>
    public class ContainerConfigurationOption
    {
        /// <summary>
        /// Name of the command line option
        /// </summary>
        /// <example>url</example>
        public string Name { get; set; }

        /// <summary>
        /// Shortened name of the command line option
        /// </summary>
        /// <example>u</example>
        public string ShortcutName { get; set; }

        /// <summary>
        /// Help text to display to the user for what this option does
        /// </summary>
        public string HelpText { get; set; }

        /// <summary>
        /// If set true the command line option will also expect a value.  If set false then it will just be a boolean flag with the prescence of the flag being True
        /// </summary>
        public bool HasValue { get; set; }
    }
}
