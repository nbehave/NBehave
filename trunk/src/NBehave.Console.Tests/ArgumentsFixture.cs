using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using gen = System.Collections.Generic;

namespace NBehave.Console.Tests
{
    [TestFixture]
    public class ArgumentsFixture
    {
        [Test]
        public void Should_show_help_with_blank_args()
        {
            Arguments arguments = new Arguments(new string[] {});

            Assert.That(arguments.ShowHelp, Is.EqualTo(true));
        }

        [Test]
        public void Should_show_help_with_null_args()
        {
            Arguments arguments = new Arguments(null);

            Assert.That(arguments.ShowHelp, Is.EqualTo(true));
        }

        [Test]
        public void Should_pull_file_names_from_first_args()
        {
            Arguments arguments = new Arguments(new string[] {"Filename.dll"});

            gen.List<string> files = new gen.List<string>(arguments.AssemblyPaths);
            Assert.That(files.Count, Is.EqualTo(1));
            Assert.That(files[0], Is.EqualTo("Filename.dll"));
        }

        [Test]
        public void Should_recognize_story_output_switch()
        {
            Arguments arguments = new Arguments(new string[] { "/storyOutput:behavior.txt" });

            Assert.That(arguments.StoryOutput, Is.EqualTo("behavior.txt"));
        }
    }
}
