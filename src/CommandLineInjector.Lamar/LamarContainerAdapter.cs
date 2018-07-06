using System;
using System.Collections.Generic;
using CommandLineInjector.Ioc;
using CommandLineInjector.Options;
using Lamar;

namespace CommandLineInjector.Lamar
{
    public class LamarContainerAdapter : ICommandContainer
    {
        private readonly IContainer _lamarContainer;
        private readonly ContainerConfigurationDelegate<INestedContainer> _extraConfiguration;

        public LamarContainerAdapter(IContainer lamarContainer, ContainerConfigurationDelegate<INestedContainer> extraConfiguration)
        {
            _lamarContainer = lamarContainer;
            _extraConfiguration = extraConfiguration;
        }

        public LamarContainerAdapter(IContainer lamarContainer)
        {
            _lamarContainer = lamarContainer;
        }

        public T GetInstance<T>() => _lamarContainer.GetInstance<T>();

        public ICommandContainer GetScoped(IEnumerable<(ContainerConfigurationOption option, string value)> universalCommands)
        {
            var scoped = _lamarContainer.GetNestedContainer();
            _extraConfiguration?.Invoke(scoped, universalCommands);
            return new LamarNestedContainerAdapter(scoped);
        }

        public void Dispose()
        {
            _lamarContainer?.Dispose();
        }
    }
}
