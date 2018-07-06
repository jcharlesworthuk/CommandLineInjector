using System;
using System.Collections.Generic;
using Autofac;
using CommandLineInjector.Ioc;
using CommandLineInjector.Options;

namespace CommandLineInjector.Autofac
{
    public class AutofacLifetimeScopeAdapter : ICommandContainer
    {
        private readonly ILifetimeScope _autofacContainer;

        public AutofacLifetimeScopeAdapter(ILifetimeScope autofacContainer)
        {
            _autofacContainer = autofacContainer;
        }

        public T GetInstance<T>() => _autofacContainer.Resolve<T>();

        public ICommandContainer GetScoped(IEnumerable<(ContainerConfigurationOption option, string value)> universalCommands)
        {
            throw new NotSupportedException("This container has already been nested once.  Further nested scopes are not supported");
        }

        public void Dispose()
        {
            _autofacContainer?.Dispose();
        }
    }
}
