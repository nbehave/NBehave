using System.IO;
using System.Linq;
using System.Text;
using NBehave.Narrator.Framework;
using NBehave.Narrator.Framework.EventListeners;
using NBehave.Narrator.Framework.EventListeners.Xml;
using NUnit.Framework;

namespace NBehave.Console.Specifications
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
            NBehaveConsoleRunner.Main(new[] { "TestPlainTextAssembly.dll", "/sf=*.scenario" });

            Assert.That(_output.ToString(), Is.StringContaining("Scenarios"));
        }

        [Test]
        public void Should_not_display_header_when_nologo_argument_set()
        {
            NBehaveConsoleRunner.Main(new[] { "TestAssembly.dll", "/nologo", "/sf=*.scenario" });

            Assert.That(_output.ToString(), Is.Not.StringContaining("Copyright"));
        }

        [Test]
        public void Should_create_colorful_listener_when_no_story_output_argument_passed_in()
        {
            var options = new ConsoleOptions(new[] { "TestAssembly.dll" });

            EventListener listener = null;
            try
            {
                listener = NBehaveConsoleRunner.CreateEventListener(options);
                var multiOutputEventListener = (MultiOutputEventListener)listener;
                Assert.That(multiOutputEventListener.Listeners[0], Is.TypeOf(typeof(ColorfulConsoleOutputEventListener)));
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
            var options = new ConsoleOptions(new[] { "TestAssembly.dll", "/storyOutput:Output.txt" });

            EventListener listener = null;
            try
            {
                listener = NBehaveConsoleRunner.CreateEventListener(options);
                var multiOutputEventListener = (MultiOutputEventListener)listener;
                Assert.That(multiOutputEventListener.Listeners[0], Is.TypeOf(typeof(TextWriterEventListener)));
            }
            finally
            {
                if (listener != null)
                    listener.RunFinished();
            }
        }

        [Test]
        public void Should_create_xml_listener_when_xml_argument_passed_in()
        {
            var options = new ConsoleOptions(new[] { "TestAssembly.dll", "/xml:XmlOutput.xml" });

            EventListener listener = null;
            try
            {
                listener = NBehaveConsoleRunner.CreateEventListener(options);
                var multiOutputEventListener = (MultiOutputEventListener)listener;
                Assert.That(multiOutputEventListener.Listeners[0], Is.TypeOf(typeof(XmlOutputEventListener)));
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
        public void Should_create_both_file_and_xml_listener_when_story_output_argument_and_xml_argument_passed_in()
        {
            var options = new ConsoleOptions(new[] { "TestAssembly.dll", "/xml:XmlOutputTest.xml", "/storyOutput:storiesTest.txt" });

            EventListener listener = null;
            try
            {
                listener = NBehaveConsoleRunner.CreateEventListener(options);
                var multiOutputEventListener = (MultiOutputEventListener)listener;
                Assert.That(multiOutputEventListener.Listeners.Count(), Is.EqualTo(2));
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
            NBehaveConsoleRunner.Main(new[] { "IDontExist.dll", "/sf=*.scenario" });
            Assert.That(_output.ToString(), Contains.Substring("File not found:"));
//            Assert.Contains("File not found: IDontExist.dll", _output.ToString());
//            Assert.IsTrue(_output.ToString().Contains("File not found: IDontExist.dll"));
        }
    }
}
