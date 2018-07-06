using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;

namespace CommandLineInjector.Tests.TestData
{
    [Description("Test Command")]
    public class TestCommand
    {
        public Task Invoke([Description("Parameter A")]string paramA, [Description("Parameter B")]int paramB, [Description("Optional Parameter")]string optional = "default value")
        {
            return Task.CompletedTask;
        }
    }
}
