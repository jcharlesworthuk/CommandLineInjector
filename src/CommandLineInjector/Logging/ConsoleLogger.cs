namespace CommandLineInjector.Logging
{
    public class ConsoleLogger : ILogger
    {
        public void Log(string message, ConsoleMessageType type = ConsoleMessageType.Unspecified)
        {
            ConsoleTrace.WriteLine(message, type);
        }
    }
}
