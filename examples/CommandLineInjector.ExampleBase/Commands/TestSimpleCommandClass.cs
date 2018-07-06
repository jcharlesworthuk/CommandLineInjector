using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using CommandLineInjector.ExampleBase.Dependencies;

namespace CommandLineInjector.ExampleBase.Commands
{
    [Description("A simple example command with injected dependencies")]
    public class TestSimpleCommandClass
    {
        private readonly IExampleService _service;

        public TestSimpleCommandClass(IExampleService service)
        {
            _service = service;
        }

        public async Task Invoke([Description("The text to print")] string text, [Description("Number of times to print")]int times = 1)
        {
            for (int i = 1; i <= times; i++)
                System.Console.WriteLine($"Your text is: {text}");

            await _service.Call();
        }
    }
}
