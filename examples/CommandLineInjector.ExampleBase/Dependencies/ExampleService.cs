using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CommandLineInjector.ExampleBase.Dependencies
{
    public class ExampleService : IExampleService
    {
        private readonly IExampleConfiguration _config;

        public ExampleService(IExampleConfiguration config)
        {
            _config = config;
        }

        public Task Call()
        {
            System.Console.WriteLine($"The service was called, it has a config value of: {_config.ConfigValue}");

            return Task.CompletedTask;
        }
    }
}
