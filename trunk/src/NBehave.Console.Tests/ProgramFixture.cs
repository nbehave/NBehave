using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using NBehave.Narrator.Framework;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using gen = System.Collections.Generic;

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
            int result = Program.Main(new string[] { "TestAssembly.dll" });

            string osVersion = Environment.OSVersion.ToString();
            string clrVersion = Environment.Version.ToString();

            string runtime = "Runtime Environment -\r\n   OS Version: " + osVersion + "\r\n  CLR Version: " + clrVersion + "\r\n";

            string expectedOutput = "NBehave version 0.3.0.0\r\nCopyright (C) 2007 Jimmy Bogard, Joe Ocampo, Morgan Persson, Tim Haughton.\r\nAll Rights Reserved.\r\n\r\n";
            expectedOutput += runtime;

            //Debt: JO Need to fix the sample project and get this test to pass.
            /*
             * Need to fix the sample project.
            expectedOutput += "\r\n..PF\r\n";
            expectedOutput += "Scenarios run: 4, Failures: 1, Pending: 1\r\n";
            expectedOutput += "\r\nFailures:\r\n1) Deposit to cash account (Savings account is in credit) FAILED\r\n  NUnit.Framework.AssertionException :   Expected: 120\r\n  But was:  100\r\n\r\n";
            */

            System.Console.Write("OutPut: " + expectedOutput);
            Assert.That(_output.ToString(), Text.StartsWith(expectedOutput));
            //Assert.That(result, Is.EqualTo(2));
        }

        [Test]
        public void Should_not_display_header_when_nologo_argument_set()
        {
            Program.Main(new string[] { "TestAssembly.dll", "/nologo" });

            string osVersion = Environment.OSVersion.ToString();
            string clrVersion = Environment.Version.ToString();

            string runtime = "Runtime Environment -\r\n   OS Version: " + osVersion + "\r\n  CLR Version: " + clrVersion + "\r\n";

            string expectedOutput = "NBehave version 0.3.0.0\r\nCopyright (C) 2007 Jimmy Bogard, Joe Ocampo, Morgan Persson, Tim Haughton.\r\nAll Rights Reserved.\r\n\r\n";
            expectedOutput += runtime;

            Assert.That(_output.ToString(), Text.DoesNotStartWith(expectedOutput));
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
    }
}
