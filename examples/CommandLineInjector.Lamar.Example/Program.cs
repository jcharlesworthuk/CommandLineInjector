using CommandLineInjector.Application;
using CommandLineInjector.ExampleBase.Commands;
using CommandLineInjector.ExampleBase.Dependencies;
using Lamar;

namespace CommandLineInjector.Lamar.Example
{
    class Program
    {
        static void Main(string[] args)
        {
            var structureMapContainer = new Container(config =>
            {
                config.For<IExampleService>().Use<ExampleService>();
                config.For<IExampleConfiguration>().Use<ExampleConfiguration>();
                config.Injectable<ExampleConfiguration>();
            });

            var containerAdapter = new LamarContainerAdapter(structureMapContainer, (container, commands) =>
            {
                container.Inject(new ExampleConfiguration(commands));
                return container;
            });

            var app = new CommandLineInjectingApplication("commandlineinjector-example", containerAdapter);

            app.RequiresCommand();
            app.AddToSubsequentAllCommands(ExampleConfiguration.ConfigValueOption);

            app.Command<TestSimpleCommandClass>("simple");
            app.Execute(args);
        }
    }
}
