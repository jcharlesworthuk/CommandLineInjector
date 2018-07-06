using System;

namespace CommandLineInjector.Logging
{
    public static class ConsoleTrace
    {
        public static void WriteLine() => System.Console.WriteLine();

        public static string ReadLine() => System.Console.ReadLine();

        public static void WriteLine(string message, ConsoleMessageType type = ConsoleMessageType.Unspecified)
        {
            var prevColour = System.Console.ForegroundColor;
            switch (type)
            {
                case ConsoleMessageType.Error:
                    System.Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case ConsoleMessageType.Warning:
                    System.Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case ConsoleMessageType.Good:
                    System.Console.ForegroundColor = ConsoleColor.DarkYellow;
                    break;
                case ConsoleMessageType.Strong:
                    System.Console.ForegroundColor = ConsoleColor.Blue;
                    break;
            }
            System.Console.WriteLine(message);
            System.Console.ForegroundColor = prevColour;
        }
    }

}
