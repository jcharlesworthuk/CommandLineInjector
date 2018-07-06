using System;
using Autofac;
using Autofac.Core;
using CommandLineInjector.Console;
using CommandLineInjector.ExampleBase.Commands;
using CommandLineInjector.ExampleBase.Dependencies;

namespace CommandLineInjector.Autofac.Example
{
    class Program
    {
        static void Main(string[] args)
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<ExampleService>().As<IExampleService>();
            builder.RegisterType<TestSimpleCommandClass>().AsSelf();

            var autofacContainer = builder.Build();

            var containerAdapter = new AutofacContainerAdapter(autofacContainer, (scopeBuilder, commands) =>
            {
                scopeBuilder.RegisterInstance(new ExampleConfiguration(commands)).As<IExampleConfiguration>();
                return scopeBuilder;
            });

            var app = new CommandLineInjectingApplication("commandlineinjector-example", containerAdapter);

            app.RequiresCommand();
            app.AddToSubsequentAllCommands(ExampleConfiguration.ConfigValueOption);

            app.Command<TestSimpleCommandClass>("simple");
            app.Execute(args);
        }
    }
}
