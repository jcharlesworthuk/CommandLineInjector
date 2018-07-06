using System;
using System.Collections.Generic;
using Autofac;
using CommandLineInjector.Ioc;
using CommandLineInjector.Options;

namespace CommandLineInjector.Autofac
{
    public class AutofacContainerAdapter : ICommandContainer
    {
        private readonly IContainer _autofacContainer;
        private readonly ContainerConfigurationDelegate<ContainerBuilder> _extraConfiguration;

        public AutofacContainerAdapter(IContainer autofacContainer, ContainerConfigurationDelegate<ContainerBuilder> extraConfiguration)
        {
            _autofacContainer = autofacContainer;
            _extraConfiguration = extraConfiguration;
        }

        public AutofacContainerAdapter(IContainer autofacContainer)
        {
            _autofacContainer = autofacContainer;
        }

        public T GetInstance<T>() => _autofacContainer.Resolve<T>();

        public ICommandContainer GetScoped(IEnumerable<(ContainerConfigurationOption option, string value)> universalCommands)
        {
            var scoped = _autofacContainer.BeginLifetimeScope(builder =>
            {
                _extraConfiguration?.Invoke(builder, universalCommands);
            });
            return new AutofacLifetimeScopeAdapter(scoped);
        }

        public void Dispose()
        {
            _autofacContainer?.Dispose();
        }
    }
}
