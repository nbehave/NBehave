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
                "Runtime Environment -{2}   OS Version: {0}{2}  CLR Version: {1}",
                Environment.OSVersion,
                Environment.Version,
                Environment.NewLine);
            writer.WriteLine(runtimeEnv);
        }

        public override object InitializeLifetimeService()
        {
            return null;
        }
    }
}
