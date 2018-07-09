using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;

namespace CommandLineInjector.Tests.TestData
{
    public class TestCommandService
    {
        [Description("Test Command One")]
        public Task CommandOne([Description("Parameter A")]string paramA, [Description("Parameter B")]int paramB, [Description("Optional Parameter")]string optional = "default value")
        {
            return Task.CompletedTask;
        }

        [Description("Test Command Two")]
        public Task CommandTwo([Description("Parameter A")]string paramA, [Description("Parameter B")]int paramB, [Description("Optional Parameter")]string optional = "default value")
        {
            return Task.CompletedTask;
        }

    }
}
