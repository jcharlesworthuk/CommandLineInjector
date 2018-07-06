using System;
using System.Collections.Generic;
using System.Text;
using CommandLineInjector.Options;

namespace CommandLineInjector.Ioc
{
    public delegate TContainer ContainerConfigurationDelegate<TContainer>(TContainer container, IEnumerable<(ContainerConfigurationOption option, string value)> universalCommands);
}
