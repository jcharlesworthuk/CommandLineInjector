using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CommandLineInjector.ExampleBase.Dependencies
{
    public interface IExampleService
    {
        Task Call();
    }
}
