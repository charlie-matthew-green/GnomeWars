using System;
using System.Threading;

namespace GnomeWars
{
    public interface ILogger
    {
        void LogLine(string output);
        void LogWithColour(string output, ConsoleColor colour);
        void WaitThenClear();
        void Wait();
    }

    public class Logger : ILogger
    {
        public void LogLine(string output)
        {
            Console.WriteLine(output);
        }

        public void LogWithColour(string output, ConsoleColor colour)
        {
            Console.ForegroundColor = colour;
            Console.Write(output);
            Console.ForegroundColor = ConsoleColor.White;
        }

        public void WaitThenClear()
        {
            Thread.Sleep(500);
            Console.Clear();
        }

        public void Wait()
        {
            Console.WriteLine($"Press return to continue...");
            Console.ReadLine();
        }
    }
}
