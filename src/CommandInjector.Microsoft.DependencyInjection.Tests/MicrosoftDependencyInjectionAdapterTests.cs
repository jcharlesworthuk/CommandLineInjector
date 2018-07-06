using System;
using System.Collections.Generic;
using System.ComponentModel;
using CommandLineInjector.Options;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Shouldly;
using Xunit;

namespace CommandInjector.Microsoft.DependencyInjection.Tests
{
    public class MicrosoftDependencyInjectionAdapterTests
    {
        [Fact]
        public void Should_Return_Nested_Container()
        {
            // Arrange
            var serviceCollection = new ServiceCollection();
            var adapter = new MicrosoftDependencyInjectionAdapter(serviceCollection, (config, commands) => config);

            // Act
            var nested = adapter.GetScoped(new List<(ContainerConfigurationOption option, string value)>());

            // Assert
            nested.ShouldNotBe(adapter);
            nested.ShouldBeOfType<MicrosoftDependencyInjectionAdapter>();
        }

        [Fact]
        public void Should_Invoke_Extra_Configuration()
        {
            // Arrange
            var serviceCollection = new ServiceCollection();
            bool configCalled = false;
            var adapter = new MicrosoftDependencyInjectionAdapter(serviceCollection, (config, commands) =>
            {
                configCalled = true;
                return config;
            });

            // Act
            adapter.GetScoped(new List<(ContainerConfigurationOption option, string value)>());

            // Assert
            configCalled.ShouldBeTrue();
        }


        [Fact]
        public void Should_Allow_Null_Extra_Configuration()
        {
            // Arrange
            var serviceCollection = new ServiceCollection();
            var adapter = new MicrosoftDependencyInjectionAdapter(serviceCollection, null);

            // Act / Assert
            Should.NotThrow(() =>  adapter.GetScoped(new List<(ContainerConfigurationOption option, string value)>()));
        }
    }
}
