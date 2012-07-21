using System;
using System.IO;

namespace NBehave.Narrator.Framework.EventListeners
{
    public interface IOutputWriter
    {
        TextWriter Out { get; }
        void ResetColor();
        void WriteLine();
        void WriteLine(string str);
        void WriteColorString(string text, ConsoleColor color);
    }
}