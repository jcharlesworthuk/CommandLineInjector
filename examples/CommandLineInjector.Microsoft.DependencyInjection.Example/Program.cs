using System;
using CommandLineInjector.Application;
using CommandLineInjector.ExampleBase.Commands;
using CommandLineInjector.ExampleBase.Dependencies;
using Microsoft.Extensions.DependencyInjection;

namespace CommandLineInjector.Microsoft.DependencyInjection.Example
{
    class Program
    {
        static void Main(string[] args)
        {
            var serviceCollection = new ServiceCollection()
                .AddTransient<IExampleService, ExampleService>()
                .AddTransient<TestSimpleCommandClass>();

            var containerAdapter = new MicrosoftDependencyInjectionAdapter(serviceCollection, (scopeBuilder, commands) =>
            {
                scopeBuilder.AddSingleton<IExampleConfiguration>(new ExampleConfiguration(commands));
                return scopeBuilder;
            });

            var app = new CommandLineInjectingApplication("commandlineinjector-example", containerAdapter);

            app.RequiresCommand();
            app.AddToSubsequentAllCommands(ExampleConfiguration.ConfigValueOption);

            app.Command<TestSimpleCommandClass>("simple");
            app.Execute(args);

            Console.ReadLine();
        }
    }
}
