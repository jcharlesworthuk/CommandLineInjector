using System;
using System.Collections.Generic;
using CommandLineInjector.Ioc;
using CommandLineInjector.Options;
using Lamar;

namespace CommandLineInjector.Lamar
{
    public class LamarNestedContainerAdapter : ICommandContainer
    {
        private readonly INestedContainer _lamarContainer;

        public LamarNestedContainerAdapter(INestedContainer lamarContainer)
        {
            _lamarContainer = lamarContainer;
        }

        public T GetInstance<T>() => _lamarContainer.GetInstance<T>();

        public ICommandContainer GetScoped(IEnumerable<(ContainerConfigurationOption option, string value)> universalCommands)
        {
            throw new NotSupportedException("This container has already been nested once.  Further nested scopes are not supported");
        }

        public void Dispose()
        {
            _lamarContainer?.Dispose();
        }
    }
}
