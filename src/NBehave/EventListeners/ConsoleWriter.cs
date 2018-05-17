using System;
using System.IO;

namespace NBehave.EventListeners
{
    public class ConsoleWriter : IOutputWriter
    {
        public TextWriter Out { get { return Console.Out; } }

        public void ResetColor()
        {
            Console.ResetColor();
        }

        public void WriteLine()
        {
            Console.WriteLine();
        }

        public void WriteLine(string str)
        {
            Console.WriteLine(str);
        }

        public void WriteColorString(string text, ConsoleColor color)
        {
            var currentColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            WriteLine(text);
            Console.ForegroundColor = currentColor;
        }
    }
}