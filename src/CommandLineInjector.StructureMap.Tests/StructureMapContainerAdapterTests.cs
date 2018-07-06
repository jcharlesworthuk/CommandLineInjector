using System;
using System.Collections.Generic;
using CommandLineInjector.Options;
using Moq;
using Shouldly;
using StructureMap;
using Xunit;

namespace CommandLineInjector.StructureMap.Tests
{
    public class StructureMapContainerAdapterTests
    {
        [Fact]
        public void GetInstance_Should_Pass_To_Container()
        {
            // Arrange
            var containerMock = new Mock<IContainer>();
            containerMock.Setup(x => x.GetInstance<string>()).Returns("test");
            var adapter = new StructureMapContainerAdapter(containerMock.Object, (config, commands) => config);

            // Act
            var result = adapter.GetInstance<string>();


            // Assert
            result.ShouldBe("test");
            containerMock.Verify(x => x.GetInstance<string>(), Times.Once());
        }

        [Fact]
        public void Should_Dispose_Container()
        {
            // Arrange
            var containerMock = new Mock<IContainer>();
            containerMock.Setup(x => x.Dispose());
            var adapter = new StructureMapContainerAdapter(containerMock.Object, (config, commands) => config);

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
            var nestedContainer = Mock.Of<IContainer>();
            containerMock.Setup(x => x.GetNestedContainer()).Returns(nestedContainer);
            var adapter = new StructureMapContainerAdapter(containerMock.Object, (config, commands) => config);

            // Act
            var nested = adapter.GetScoped(new List<(ContainerConfigurationOption option, string value)>());

            // Assert
            nested.ShouldNotBe(adapter);
            nested.ShouldBeOfType<StructureMapContainerAdapter>();
            containerMock.Verify(x => x.GetNestedContainer(), Times.Once());
        }

        [Fact]
        public void Should_Invoke_Extra_Configuration()
        {
            // Arrange
            var containerMock = new Mock<IContainer>();
            var nestedContainer = Mock.Of<IContainer>();
            containerMock.Setup(x => x.GetNestedContainer()).Returns(nestedContainer);
            bool configCalled = false;
            var adapter = new StructureMapContainerAdapter(containerMock.Object, (config, commands) =>
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
            var containerMock = new Mock<IContainer>();
            var nestedContainer = Mock.Of<IContainer>();
            containerMock.Setup(x => x.GetNestedContainer()).Returns(nestedContainer);
            var adapter = new StructureMapContainerAdapter(containerMock.Object, null);

            // Act / Assert
            Should.NotThrow(() =>  adapter.GetScoped(new List<(ContainerConfigurationOption option, string value)>()));
        }
    }
}
