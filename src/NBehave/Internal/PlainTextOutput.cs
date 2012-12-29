using System;
using System.IO;
using System.Reflection;

namespace NBehave.Internal
{
    public class PlainTextOutput : MarshalByRefObject
    {
        private readonly TextWriter writer;

        public PlainTextOutput(TextWriter writer)
        {
            this.writer = writer;
        }

        public void WriteLine(string text)
        {
            writer.WriteLine(text);
        }

        public void WriteHeader()
        {
            var executingAssembly = Assembly.GetExecutingAssembly();
            var semVer = (AssemblyInformationalVersionAttribute)
                         Attribute.GetCustomAttribute(executingAssembly, typeof(AssemblyInformationalVersionAttribute));
            var copyrights = (AssemblyCopyrightAttribute[])
                Attribute.GetCustomAttributes(executingAssembly, typeof(AssemblyCopyrightAttribute));

            writer.WriteLine("NBehave version {0}", semVer.InformationalVersion);

            foreach (var copyrightAttribute in copyrights)
            {
                writer.WriteLine(copyrightAttribute.Copyright);
            }

            if (copyrights.Length > 0)
            {
                writer.WriteLine("All Rights Reserved.");
            }
        }

        public void WriteSeparator()
        {
            writer.WriteLine(string.Empty);
        }

        public void WriteRuntimeEnvironment()
        {
            var runtimeEnv = string.Format(
                "Runtime Environment -\r\n   OS Version: {0}\r\n  CLR Version: {1}",
                Environment.OSVersion,
                Environment.Version);
            writer.WriteLine(runtimeEnv);
        }

        public override object InitializeLifetimeService()
        {
            return null;
        }
    }
}
