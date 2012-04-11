// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PlainTextOutput.cs" company="NBehave">
//   Copyright (c) 2007, NBehave - http://nbehave.codeplex.com/license
// </copyright>
// <summary>
//   Defines the PlainTextOutput type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.IO;
using System.Reflection;

namespace NBehave.Narrator.Framework.Internal
{
    public class PlainTextOutput : MarshalByRefObject
    {
        private readonly TextWriter _writer;

        public PlainTextOutput(TextWriter writer)
        {
            _writer = writer;
        }

        public void WriteLine(string text)
        {
            _writer.WriteLine(text);
        }

        public void WriteHeader()
        {
            var executingAssembly = Assembly.GetExecutingAssembly();
            var version = executingAssembly.GetName().Version;

            var copyrights = (AssemblyCopyrightAttribute[])
                Attribute.GetCustomAttributes(executingAssembly, typeof(AssemblyCopyrightAttribute));

            _writer.WriteLine("NBehave version {0}", version);

            foreach (var copyrightAttribute in copyrights)
            {
                _writer.WriteLine(copyrightAttribute.Copyright);
            }

            if (copyrights.Length > 0)
            {
                _writer.WriteLine("All Rights Reserved.");
            }
        }

        public void WriteSeparator()
        {
            _writer.WriteLine(string.Empty);
        }

        public void WriteRuntimeEnvironment()
        {
            var runtimeEnv = string.Format(
                "Runtime Environment -\r\n   OS Version: {0}\r\n  CLR Version: {1}",
                Environment.OSVersion,
                Environment.Version);
            _writer.WriteLine(runtimeEnv);
        }

        public override object InitializeLifetimeService()
        {
            return null;
        }
    }
}
