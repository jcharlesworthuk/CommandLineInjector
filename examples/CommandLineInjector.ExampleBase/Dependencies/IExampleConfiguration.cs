using System;
using System.Collections.Generic;
using System.Text;

namespace CommandLineInjector.ExampleBase.Dependencies
{
    public interface IExampleConfiguration
    {
        string ConfigValue { get; }
    }
}
