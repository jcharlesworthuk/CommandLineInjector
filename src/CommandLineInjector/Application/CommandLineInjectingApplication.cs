using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using CommandLineInjector.Extensions;
using CommandLineInjector.Ioc;
using CommandLineInjector.Options;
using Microsoft.Extensions.CommandLineUtils;

namespace CommandLineInjector.Application
{
    /// <summary>
    /// Inherits from <see cref="CommandLineApplication"/> and adds support for Dependency Injection and automatic method parameter binding
    /// </summary>
    public class CommandLineInjectingApplication : CommandLineApplication
    {
        private readonly ICommandContainer _container;

        /// <summary>
        /// Creates a new instance of this inhjecting command line application
        /// </summary>
        /// <param name="name">Application name</param>
        /// <param name="container">DI container adapter for your chosen DI framework</param>
        public CommandLineInjectingApplication(string name, ICommandContainer container)
        {
            _container = container;
            Name = name;
            HelpOption("-?|-h|--help");
        }

        /// <summary>
        /// Adds a default OnExecute implementation instructing the caller to specify a command name
        /// </summary>
        public CommandLineInjectingApplication RequiresCommand()
        {
            OnExecute(() =>
            {
                System.Console.WriteLine("No command.");
                System.Console.WriteLine("Specify --help for a list of available options and commands.");
                return 0;
            });

            return this;
        }

        private readonly List<ContainerConfigurationOption> _universalCommandOptions = new List<ContainerConfigurationOption>();

        /// <summary>
        /// Adds this global option to all subsequent commands added with <see cref="Command{T}(string)"/> or <see cref="CommandService{T}(string[])"/>
        /// </summary>
        /// <param name="option"></param>
        /// <returns></returns>
        public CommandLineInjectingApplication AddToSubsequentAllCommands(ContainerConfigurationOption option)
        {
            _universalCommandOptions.Add(option);

            return this;
        }

        /// <summary>
        /// Add this class as a command to the application
        /// </summary>
        /// <typeparam name="TCommandType">Type to be resolved when this command is executed</typeparam>
        /// <param name="name">Command/method name</param>
        /// <remarks>Make sure your command type is registered with your DI container</remarks>
        public CommandLineInjectingApplication Command<TCommandType>(string name)
        {
            Command(name, config =>
            {
                var invokeMethod = typeof(TCommandType).FindMethodOrDefault(name);
                var indexedParameters = invokeMethod.GetParameters().IndexShortenedNames(new HashSet<string>(_universalCommandOptions.Select(x => x.ShortcutName)));
                var options = new Dictionary<ParameterInfo, CommandOption>();
                var arguments = new Dictionary<ParameterInfo, CommandArgument>();
                foreach (var parameterKvp in indexedParameters)
                {
                    var paramName = parameterKvp.Value.Name;
                    string paramShortName = parameterKvp.Key;
                    var description = parameterKvp.Value.GetHelpText();
                    if (parameterKvp.Value.HasDefaultValue)
                    {
                        var option = parameterKvp.Value.RequiresPassedValue()
                            ? config.Option($"-{paramShortName}|--{paramName} <value>", description, CommandOptionType.SingleValue)
                            : config.Option($"-{paramShortName}|--{paramName}", description, CommandOptionType.NoValue);
                        options.Add(parameterKvp.Value, option);
                    }
                    else
                    {
                        var argument = config.Argument($"[{paramName}]", description);
                        arguments.Add(parameterKvp.Value, argument);
                    }
                }

                config.Description = typeof(TCommandType).GetDescription();
                config.HelpOption("-?|-h|--help");

                List<(ContainerConfigurationOption ConfigOption, CommandOption CommandOption)> universalCommands = _universalCommandOptions.Select(opt =>
                {
                    var option = opt.HasValue
                        ? config.Option($"-{opt.ShortcutName}|--{opt.Name} <value>", opt.HelpText, CommandOptionType.SingleValue)
                        : config.Option($"-{opt.ShortcutName}|--{opt.Name}", opt.HelpText, CommandOptionType.NoValue);

                    return (opt, option);
                }).ToList();

                config.OnExecute(async () =>
                {
                    var argumentValues = new List<object>();
                    foreach (var parameterKvp in indexedParameters)
                    {
                        if (arguments.ContainsKey(parameterKvp.Value))
                        {
                            var argValueString = arguments[parameterKvp.Value].Value;
                            var typed = Convert.ChangeType(argValueString, parameterKvp.Value.ParameterType);
                            argumentValues.Add(typed);
                        }
                        else if (options.ContainsKey(parameterKvp.Value))
                        {
                            if (!options[parameterKvp.Value].HasValue())
                                argumentValues.Add(parameterKvp.Value.DefaultValue);
                            else
                            {
                                var argValueString = options[parameterKvp.Value].Value();
                                var typed = Convert.ChangeType(argValueString, parameterKvp.Value.ParameterType);
                                argumentValues.Add(typed);
                            }
                        }
                    }

                    using (var scopedContainer = _container.GetScoped(universalCommands.Where(x => x.CommandOption.HasValue()).Select(opt => (opt.ConfigOption, opt.CommandOption.Value()))))
                    {
                        var command = scopedContainer.GetInstance<TCommandType>();

                        var task = (Task)invokeMethod.Invoke(command, argumentValues.ToArray());
                        
                        await task.ConfigureAwait(true);
                    }
                    return 0;
                });
            });

            return this;
        }


        /// <summary>
        /// Add the specified methods from this class as commands in the application
        /// </summary>
        /// <typeparam name="TCommandType">Type to be resolved when any of these commands are executed</typeparam>
        /// <param name="names">Method names on the command type</param>s
        /// <remarks>Make sure your command type is registered with your DI container</remarks>
        /// <example>CommandService&lt;MyServiceType&gt;(nameof(MyServiceType.CommandMethod))</example>
        public CommandLineInjectingApplication CommandService<TCommandType>(params string[] names)
        {
            foreach (var name in names.Select(x => x.LowercaseFirstChar()))
            {
                Command<TCommandType>(name);
            }

            return this;
        }

    }
}
