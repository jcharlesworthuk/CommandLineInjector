using System;
using System.Collections.Generic;
using System.ComponentModel;
using CommandLineInjector.Ioc;
using CommandLineInjector.Options;
using Microsoft.Extensions.DependencyInjection;

namespace CommandInjector.Microsoft.DependencyInjection
{
    public class MicrosoftDependencyInjectionAdapter: ICommandContainer
    {
        private readonly IServiceCollection _serviceCollection;
        private IServiceProvider _serviceProvider = null;
        private readonly ContainerConfigurationDelegate<IServiceCollection> _extraConfiguration;

        public MicrosoftDependencyInjectionAdapter(IServiceCollection serviceCollection, ContainerConfigurationDelegate<IServiceCollection> extraConfiguration)
        {
            _serviceCollection = serviceCollection;
            _extraConfiguration = extraConfiguration;
        }

        public MicrosoftDependencyInjectionAdapter(IServiceProvider serviceProvider, IServiceCollection serviceCollection)
        {
            _serviceProvider = serviceProvider;
            _serviceCollection = serviceCollection;
        }

        public T GetInstance<T>() => (_serviceProvider ?? (_serviceProvider = _serviceCollection.BuildServiceProvider())).GetService<T>();

        public ICommandContainer GetScoped(IEnumerable<(ContainerConfigurationOption option, string value)> universalCommands)
        {
            _extraConfiguration?.Invoke(_serviceCollection, universalCommands);
            var scoped = _serviceCollection.BuildServiceProvider();
            return new MicrosoftDependencyInjectionAdapter(scoped, _serviceCollection);
        }

        public void Dispose()
        { }
    }
}
