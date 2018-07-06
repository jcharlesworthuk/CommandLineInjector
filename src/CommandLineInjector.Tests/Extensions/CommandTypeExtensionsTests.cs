using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using CommandLineInjector.Exceptions;
using CommandLineInjector.Extensions;
using CommandLineInjector.Tests.TestData;
using Shouldly;
using Xunit;

namespace CommandLineInjector.Tests.Extensions
{
    public class CommandTypeExtensionsTests
    {
        public class TestCommandWithoutDescription
        {
        }

        [InlineData(typeof(TestCommand), "Test Command")]
        [InlineData(typeof(TestCommandWithoutDescription), "Test Command Without Description")]
        [Theory]
        public void Should_Get_Correct_Description(Type commandType, string expectedDescrpition)
        {
            // Act
            var result = commandType.GetDescription();

            // Assert
            result.ShouldBe(expectedDescrpition);
        }


        public class TestCommandWithParameterlessInvoke
        {
            public Task Invoke()
            {
                return Task.CompletedTask;
            }
        }

        [InlineData(typeof(TestCommand))]
        [InlineData(typeof(TestCommandWithParameterlessInvoke))]
        [Theory]
        public void Should_Find_Invoke_Method(Type commandType)
        {
            // Act
            var result = commandType.FindInvokeMethod();

            // Assert
            result.ShouldNotBeNull();
            result.Name.ShouldBe(nameof(TestCommand.Invoke));
        }


        public class TestCommandWithNoInvokeMethod
        {
        }

        [InlineData(typeof(TestCommandWithNoInvokeMethod))]
        [Theory]
        public void Should_Throw_If_Cant_Find_Invoke_Method(Type commandType)
        {
            // Act / Assert
            Should.Throw<InvalidCommandTypeException>(() => commandType.FindInvokeMethod());
        }
    }
}
