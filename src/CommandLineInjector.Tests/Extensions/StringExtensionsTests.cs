using System;
using System.Collections.Generic;
using System.Text;
using CommandLineInjector.Extensions;
using Shouldly;
using Xunit;

namespace CommandLineInjector.Tests.Extensions
{
    public class StringExtensionsTests
    {
        [InlineData("SomeStringInPascalCase", "Some String In Pascal Case")]
        [InlineData("AnotherMuchLongerStringInPascalCase", "Another Much Longer String In Pascal Case")]
        [Theory]
        public void Should_Split_Pascal_Case_Strings(string source, string expectedResult)
        {
            // Act
            var result = source.SplitOutPascalCase();

            // Assert
            result.ShouldBe(expectedResult);
        }

        [InlineData("someStringInCamelCase", "some String In Camel Case")]
        [InlineData("anotherMuchLongerStringInCamelCase", "another Much Longer String In Camel Case")]
        [Theory]
        public void Should_Split_Camel_Case_Strings(string source, string expectedResult)
        {
            // Act
            var result = source.SplitOutPascalCase();

            // Assert
            result.ShouldBe(expectedResult);
        }

        [InlineData("SomeString   With    Spaces", "Some String With Spaces")]
        [InlineData("Another StringWith  ManyMore      Spaces", "Another String With Many More Spaces")]
        [InlineData("       String With Leading Spaces", "String With Leading Spaces")]
        [InlineData("String With Trailing Spaces     ", "String With Trailing Spaces")]
        [Theory]
        public void Should_Remove_Extra_Spaces(string source, string expectedResult)
        {
            // Act
            var result = source.SplitOutPascalCase();

            // Assert
            result.ShouldBe(expectedResult);
        }


    }
}
