using System;
using System.Collections.Generic;
using System.Text;

namespace CommandLineInjector.Exceptions
{
    public class InvalidCommandTypeException : Exception
    {
        public InvalidCommandTypeException(Type commandType)
            : base($"The command type {commandType.Name} is not valid")
        { }

        public InvalidCommandTypeException(Type commandType, string message)
            : base($"{message} (command type {commandType.Name}")
        { }
    }
}
