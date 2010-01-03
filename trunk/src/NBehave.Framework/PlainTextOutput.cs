using System;
using System.IO;
using System.Reflection;

namespace NBehave.Narrator.Framework
{
	public class PlainTextOutput
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
			Assembly executingAssembly = Assembly.GetExecutingAssembly();
			Version version = executingAssembly.GetName().Version;

			var copyrights = (AssemblyCopyrightAttribute[])
				Attribute.GetCustomAttributes(executingAssembly, typeof(AssemblyCopyrightAttribute));

			_writer.WriteLine("NBehave version {0}", version);

			foreach (AssemblyCopyrightAttribute copyrightAttribute in copyrights)
			{
				_writer.WriteLine(copyrightAttribute.Copyright);
			}

			if (copyrights.Length > 0)
				_writer.WriteLine("All Rights Reserved.");
		}
		
		public void WriteSeparator()
		{
			_writer.WriteLine("");
		}

		public void WriteRuntimeEnvironment()
		{
			string runtimeEnv =
				string.Format("Runtime Environment -\r\n   OS Version: {0}\r\n  CLR Version: {1}", Environment.OSVersion,
				              Environment.Version);
			_writer.WriteLine(runtimeEnv);
		}
	}
}
