using System;
using System.IO;
using System.Text;
using NBehave.Narrator.Framework.EventListeners;
using NBehave.VS2010.Plugin.Contracts;

namespace NBehave.VS2010.Plugin.Domain
{
    public class OutputWindowWriter : TextWriter, IOutputWriter
    {
        private readonly IOutputWindow outputWindow;

        public OutputWindowWriter(IOutputWindow outputWindow)
        {
            this.outputWindow = outputWindow;
        }

        public TextWriter Out
        {
            get { return this; }
        }

        public void ResetColor()
        { }

        public override void WriteLine()
        {
            WriteLine("");
        }

        public override void WriteLine(string value)
        {
            outputWindow.WriteLine(value);
        }

        public void WriteColorString(string text, ConsoleColor color)
        {
            WriteLine(text);
        }

        public override Encoding Encoding
        {
            get { return Encoding.Unicode; }
        }
    }
}