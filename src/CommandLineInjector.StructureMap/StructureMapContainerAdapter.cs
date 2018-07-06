using System;
using System.Collections.Generic;
using CommandLineInjector.Ioc;
using CommandLineInjector.Logging;
using CommandLineInjector.Options;
using StructureMap;

namespace CommandLineInjector.StructureMap
{
    public class StructureMapContainerAdapter : ICommandContainer
    {
        private readonly IContainer _structureMapContainer;
        private readonly ContainerConfigurationDelegate<IContainer> _extraConfiguration;

        public StructureMapContainerAdapter(IContainer structureMapContainer, ContainerConfigurationDelegate<IContainer> extraConfiguration)
        {
            _structureMapContainer = structureMapContainer;
            _extraConfiguration = extraConfiguration;
        }

        public StructureMapContainerAdapter(IContainer structureMapContainer)
        {
            _structureMapContainer = structureMapContainer;
        }

        public T GetInstance<T>() => _structureMapContainer.GetInstance<T>();

        public ICommandContainer GetScoped(IEnumerable<(ContainerConfigurationOption option, string value)> universalCommands)
        {
            var scoped = _structureMapContainer.GetNestedContainer();
            _extraConfiguration?.Invoke(scoped, universalCommands);
            return new StructureMapContainerAdapter(scoped);
        }

        public void Dispose()
        {
            _structureMapContainer?.Dispose();
        }
    }
}
