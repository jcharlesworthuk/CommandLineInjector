using System;
using CommandLineInjector.Application;
using CommandLineInjector.ExampleBase.Commands;
using CommandLineInjector.ExampleBase.Dependencies;
using StructureMap;

namespace CommandLineInjector.StructureMap.Example
{
    class Program
    {
        static void Main(string[] args)
        {
            var structureMapContainer = new Container(config =>
            {
                config.For<IExampleService>().Use<ExampleService>();
            });

            var containerAdapter = new StructureMapContainerAdapter(structureMapContainer, (container, commands) =>
            {
                container.Configure(config =>
                {
                    config.For<IExampleConfiguration>().Use(new ExampleConfiguration(commands));
                });
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
