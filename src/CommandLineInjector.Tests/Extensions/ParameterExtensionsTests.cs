using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using CommandLineInjector.Extensions;
using CommandLineInjector.Tests.TestData;
using Shouldly;
using Xunit;

namespace CommandLineInjector.Tests.Extensions
{
    public class ParameterExtensionsTests
    {
        [Fact]
        public void Should_Index_Parameters()
        {
            // Arrange
            var parameters = typeof(TestCommand).GetMethod(nameof(TestCommand.Invoke)).GetParameters();

            // Act
            var result = parameters.IndexShortenedNames(new HashSet<string>());

            // Assert
            var keys = result.Keys.ToList();
            keys.Count.ShouldBe(3);
            keys[0].ShouldBe("paramA");
            keys[1].ShouldBe("paramB");
            keys[2].ShouldBe("o");
        }

        [Fact]
        public void Should_Index_Parameters_Ignore_Specific_Keys()
        {
            // Arrange
            var parameters = typeof(TestCommand).GetMethod(nameof(TestCommand.Invoke)).GetParameters();

            // Act
            var result = parameters.IndexShortenedNames(new HashSet<string>(new string[]{"o", "op"}));

            // Assert
            var keys = result.Keys.ToList();
            keys[2].ShouldBe("opt");
        }

        [Fact]
        public void Should_Should_Get_Parameter_Description()
        {
            // Arrange
            var parameter = typeof(TestCommand).GetMethod(nameof(TestCommand.Invoke)).GetParameters().First();

            // Act
            var description = parameter.GetHelpText();

            // Assert
            description.ShouldBe("Parameter A");
        }

        [Fact]
        public void Should_Require_Passed_Value_For_String()
        {
            // Arrange
            var parameter = typeof(TestCommand).GetMethod(nameof(TestCommand.Invoke)).GetParameters().First();

            // Act
            var result = parameter.RequiresPassedValue();

            // Assert
            result.ShouldBeTrue();
        }

    }
}
