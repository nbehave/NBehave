using System.IO;
using System.Text;
using NUnit.Framework;

namespace NBehave.Console.Specifications
{
    [TestFixture]
    public class ConsoleOptionsSpec
    {
        public class When_validating : ConsoleOptionsSpec
        {
            [Test]
            public void scenarioFiles_is_required()
            {
                var c = new ConsoleOptions();
                Assert.IsFalse(c.IsValid());
            }

            [Test]
            public void Should_have_at_least_one_default_parameter()
            {
                var c = new ConsoleOptions{ ScenarioFiles = "foo.txt"};
                Assert.IsFalse(c.IsValid());
            }

            [Test]
            public void should_have_out_file_if_dry_run()
            {
                var c = new ConsoleOptions { ScenarioFiles = "foo.txt", DryRun = true};
                Assert.IsFalse(c.IsValid());
            }

            [Test]
            public void should_be_valid_if_help_is_set()
            {
                var c = new ConsoleOptions { Help = true};
                Assert.IsTrue(c.IsValid());
            }
        }

        public class When_printing_help : ConsoleOptionsSpec
        {
            private TextWriter _original;
            private string _output;

            [SetUp]
            public void When_printing_help_text()
            {
                _original = System.Console.Out;
                var output = new StringBuilder();
                TextWriter writer = new StringWriter(output);
                System.Console.SetOut(writer);
                var c = new ConsoleOptions();
                c.ShowHelp();
                _output = output.ToString();
            }

            [TearDown]
            public void Switch_back_console_out()
            {
                if (_original != null)
                    System.Console.SetOut(_original);
            }

            [Test]
            public void Should_display_description_of_parameter()
            {
                Assert.That(_output, Does.Contain("/scenarioFiles=STR    Scenario files(s) to run. Ex: scenarioFiles:scenario1.txt;scenario2.txt (Short format: /sf=STR)"));
            }

            [Test]
            public void Should_display_description_of_boolean_parameter()
            {
                Assert.That(_output, Does.Contain("/nologo               Do not display the logo"));
            }
        }
    }
}