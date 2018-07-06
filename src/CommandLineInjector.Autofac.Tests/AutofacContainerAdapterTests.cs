using System;
using System.Collections.Generic;
using Autofac;
using CommandLineInjector.Options;
using Moq;
using Shouldly;
using Xunit;

namespace CommandLineInjector.Autofac.Tests
{
    public class AutofacContainerAdapterTests
    {

        [Fact]
        public void Should_Dispose_Container()
        {
            // Arrange
            var containerMock = new Mock<IContainer>();
            containerMock.Setup(x => x.Dispose());
            var adapter = new AutofacContainerAdapter(containerMock.Object, (config, commands) => config);

            // Act
            adapter.Dispose();

            // Assert
            containerMock.Verify(x => x.Dispose(), Times.Once());
        }

        [Fact]
        public void Should_Return_Nested_Container()
        {
            // Arrange
            var containerMock = new Mock<IContainer>();
            var lifetimeScope = Mock.Of<ILifetimeScope>();
            containerMock.Setup(x => x.BeginLifetimeScope(It.IsAny<Action<ContainerBuilder>>())).Returns(lifetimeScope);
            var adapter = new AutofacContainerAdapter(containerMock.Object, (config, commands) => config);

            // Act
            var nested = adapter.GetScoped(new List<(ContainerConfigurationOption option, string value)>());

            // Assert
            nested.ShouldNotBe(adapter);
            nested.ShouldBeOfType<AutofacLifetimeScopeAdapter>();
            containerMock.Verify(x => x.BeginLifetimeScope(It.IsAny<Action<ContainerBuilder>>()), Times.Once());
        }

        [Fact]
        public void Should_Invoke_Extra_Configuration()
        {
            // Arrange
            var containerMock = new Mock<IContainer>();
            var lifetimeScope = Mock.Of<ILifetimeScope>();
            Action<ContainerBuilder> configAction = null;
            containerMock.Setup(x => x.BeginLifetimeScope(It.IsAny<Action<ContainerBuilder>>())).Callback<Action<ContainerBuilder>>(action => { configAction = action; }).Returns(lifetimeScope);
            bool configCalled = false;
            var adapter = new AutofacContainerAdapter(containerMock.Object, (config, commands) =>
            {
                configCalled = true;
                return config;
            });

            // Act
            adapter.GetScoped(new List<(ContainerConfigurationOption option, string value)>());
            configAction(null);

            // Assert
            configCalled.ShouldBeTrue();
        }


        [Fact]
        public void Should_Allow_Null_Extra_Configuration()
        {
            // Arrange
            var containerMock = new Mock<IContainer>();
            var lifetimeScope = Mock.Of<ILifetimeScope>();
            Action<ContainerBuilder> configAction = null;
            containerMock.Setup(x => x.BeginLifetimeScope(It.IsAny<Action<ContainerBuilder>>())).Callback<Action<ContainerBuilder>>(action => { configAction = action; }).Returns(lifetimeScope);
            var adapter = new AutofacContainerAdapter(containerMock.Object, null);

            // Act / Assert
            Should.NotThrow(() =>
            {
                adapter.GetScoped(new List<(ContainerConfigurationOption option, string value)>());
                configAction(null);
            });
        }
    }
}
