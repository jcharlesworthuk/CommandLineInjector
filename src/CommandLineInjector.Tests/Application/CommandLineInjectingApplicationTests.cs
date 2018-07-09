using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommandLineInjector.Application;
using CommandLineInjector.Tests.TestData;
using Shouldly;
using Xunit;

namespace CommandLineInjector.Tests.Application
{
    public class CommandLineInjectingApplicationTests
    {
        [Fact]
        public void Should_Add_Help_Option()
        {
            // Act
            var app = new CommandLineInjectingApplication(null, null);

            // Assert
            app.OptionHelp.ShouldNotBeNull();
        }

        [Fact]
        public void Should_Set_Name()
        {
            // Act
            var app = new CommandLineInjectingApplication("test name", null);

            // Assert
            app.Name.ShouldBe("test name");
        }

        [Fact]
        public void Should_Set_Invoke_For_RequiresCommand()
        {
            // Arrange
            var app = new CommandLineInjectingApplication(null, null);
            var defaultInvoke = app.Invoke;

            // Act
            app.RequiresCommand();

            // Assert
            app.Invoke.ShouldNotBeNull();
            app.Invoke.ShouldNotBe(defaultInvoke);
        }

        [Fact]
        public void Should_Set_Command()
        {
            // Arrange
            var app = new CommandLineInjectingApplication(null, null);

            // Act
            app.Command<TestCommand>("command name");

            // Assert
            app.Commands.Count.ShouldBe(1);
        }


        [Fact]
        public void Should_Set_Invoke_Method_On_New_Command()
        {
            // Arrange
            var app = new CommandLineInjectingApplication(null, null);

            // Act
            app.Command<TestCommand>("command name");

            // Assert
            app.Commands.Last().Invoke.ShouldNotBeNull();
        }


        [Fact]
        public void Should_Set_Command_Service_Commands()
        {
            // Arrange
            var app = new CommandLineInjectingApplication(null, null);

            // Act
            app.CommandService<TestCommandService>(nameof(TestCommandService.CommandOne), nameof(TestCommandService.CommandTwo));

            // Assert
            app.Commands.Count.ShouldBe(2);
        }
        
        [Fact]
        public void Should_Set_Names_On_Command_Service_Commands()
        {
            // Arrange
            var app = new CommandLineInjectingApplication(null, null);

            // Act
            app.CommandService<TestCommandService>(nameof(TestCommandService.CommandOne), nameof(TestCommandService.CommandTwo));

            // Assert
            app.Commands[0].Name.ShouldBe("commandOne");
            app.Commands[1].Name.ShouldBe("commandTwo");
        }

        [Fact]
        public void Should_Set_Invoke_Methods_On_Command_Service_Commands()
        {
            // Arrange
            var app = new CommandLineInjectingApplication(null, null);

            // Act
            app.CommandService<TestCommandService>(nameof(TestCommandService.CommandOne), nameof(TestCommandService.CommandTwo));

            // Assert
            app.Commands[0].Invoke.ShouldNotBeNull();
            app.Commands[1].Invoke.ShouldNotBeNull();
        }
    }
}
