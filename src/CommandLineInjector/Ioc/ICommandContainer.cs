using System;
using System.Collections.Generic;
using System.Text;
using CommandLineInjector.Options;

namespace CommandLineInjector.Ioc
{
    /// <summary>
    /// IOC container that is used to resolve the commands
    /// </summary>
    public interface ICommandContainer : IDisposable
    {
        /// <summary>
        /// Creates or finds the default instance of <typeparamref name="T" />.
        /// </summary>
        /// <typeparam name="T">The type which instance is to be created or found.</typeparam>
        /// <returns>The default instance of <typeparamref name="T" />.</returns>
        T GetInstance<T>();

        /// <summary>
        /// Creates a new nested/scoped container using the values of these command line arguments
        /// </summary>
        /// <returns>The created nested container.</returns>
        ICommandContainer GetScoped(IEnumerable<(ContainerConfigurationOption option, string value)> universalCommands);

    }
}
