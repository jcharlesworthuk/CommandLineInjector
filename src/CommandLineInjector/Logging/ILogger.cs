using System;
using System.Collections.Generic;
using System.Text;

namespace CommandLineInjector.Logging
{
    public interface ILogger
    {
        void Log(string message, ConsoleMessageType type = ConsoleMessageType.Unspecified);
    }
}
