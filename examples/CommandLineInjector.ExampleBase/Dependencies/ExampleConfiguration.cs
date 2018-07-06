using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommandLineInjector.Options;

namespace CommandLineInjector.ExampleBase.Dependencies
{
    public class ExampleConfiguration : IExampleConfiguration
    {
        public ExampleConfiguration(string configValue)
        {
            ConfigValue = configValue;
        }

        public ExampleConfiguration(IEnumerable<(ContainerConfigurationOption option, string value)> universalCommands)
        {
            ConfigValue = universalCommands.Aggregate("default value", (acc, val) => val.option.Name == ConfigValueOption.Name ? val.value : acc);
        }

        public static ContainerConfigurationOption ConfigValueOption { get; } = new ContainerConfigurationOption
        {
            Name = "configValue",
            ShortcutName = "c",
            HelpText = "Set the config value",
            HasValue = true
        };

        public string ConfigValue { get; }
    }
}
