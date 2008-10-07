using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using NBehave.Narrator.Framework;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using gen = System.Collections.Generic;
using NBehave.Narrator.Framework.EventListeners;
using NBehave.Narrator.Framework.EventListeners.Xml;


namespace NBehave.Console.Tests
{
    [TestFixture]
    public class ProgramFixture
    {
        private TextWriter _original;
        private StringBuilder _output;

        [SetUp]
        public void Switch_console_out()
        {
            _original = System.Console.Out;

            _output = new StringBuilder();
            TextWriter writer = new StringWriter(_output);
            System.Console.SetOut(writer);
        }

        [TearDown]
        public void Switch_back_console_out()
        {
            if (_original != null)
                System.Console.SetOut(_original);
        }

        [Test]
        public void Should_run_example_framework_correctly()
        {
            Program.Main(new[] { "TestAssembly.dll" });

            Assert.That(_output.ToString(), Text.Contains("Scenarios"));
        }

        [Test]
        public void Should_not_display_header_when_nologo_argument_set()
        {
            Program.Main(new[] { "TestAssembly.dll", "/nologo" });

            Assert.That(_output.ToString(), Text.DoesNotContain("Copyright"));
        }

        [Test]
        public void Should_create_empty_listener_when_no_story_output_argument_passed_in()
        {
            ConsoleOptions options = new ConsoleOptions(new string[] { "TestAssembly.dll" });

            IEventListener listener = null;
            try
            {
                listener = Program.CreateEventListener(options);
                Assert.That(listener, Is.Not.TypeOf(typeof(FileOutputEventListener)));
            }
            finally
            {
                if (listener != null)
                    listener.RunFinished();
            }
        }

        [Test]
        public void Should_create_file_listener_when_story_output_argument_passed_in()
        {
            ConsoleOptions options = new ConsoleOptions(new string[] { "TestAssembly.dll", "/storyOutput:Output.txt" });

            IEventListener listener = null;
            try
            {
                listener = Program.CreateEventListener(options);
                Assert.That(listener, Is.TypeOf(typeof(FileOutputEventListener)));
            }
            finally
            {
                if (listener != null)
                    listener.RunFinished();
            }
        }

        [Test]
        public void Should_create_xml_listener_when_story_output_argument_passed_in()
        {
            ConsoleOptions options = new ConsoleOptions(new string[] { "TestAssembly.dll", "/xml:XmlOutput.xml" });

            IEventListener listener = null;
            try
            {
                listener = Program.CreateEventListener(options);
                Assert.That(listener, Is.TypeOf(typeof(XmlOutputEventListener)));
            }
            finally
            {
                if (listener != null)
                {
                    listener.RunStarted();
                    listener.RunFinished();
                }
            }
        }

        [Test]
        public void Should_display_errormessage_if_assembly_doesnt_exist()
        {
            Program.Main(new string[] { "IDontExist.dll" });
			Assert.IsTrue(_output.ToString().Contains("File not found: IDontExist.dll"));
        }
    }
}
