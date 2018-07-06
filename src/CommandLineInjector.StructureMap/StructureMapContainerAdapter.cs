using System;
using System.Collections.Generic;
using System.ComponentModel;
using CommandLineInjector.Ioc;
using CommandLineInjector.Logging;
using CommandLineInjector.Options;

namespace CommandLineInjector.StructureMap
{
    public class StructureMapContainerAdapter : ICommandContainer
    {
        private readonly IContainer _structureMapContainer;

        public StructureMapContainerAdapter(IContainer structureMapContainer)
        {
            _structureMapContainer = structureMapContainer;
        }

        public T GetInstance<T>() => _structureMapContainer.GetInstance<T>();

        public ICommandContainer GetScoped(IEnumerable<(ContainerConfigurationOption option, string value)> universalCommands)
        {
            var scoped = _structureMapContainer.GetNestedContainer();
            var configManager = new CommandLineConfigManager(universalCommands, scoped.GetInstance<ILogger>());
            scoped.Configure(cfg =>
            {
                cfg.For<IWymConfigurationManager>().Use(configManager).Singleton();
                cfg.For<HttpClientSingletonWrapper>().Use(new HttpClientSingletonWrapper(configManager)).Singleton();
            });
            return new StructureMapContainerAdapter(scoped);
        }

        public void Dispose()
        {
            _structureMapContainer?.Dispose();
        }
    }
}
