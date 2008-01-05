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
    public class ConsoleOutputFixture
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
        public void Should_output_header()
        {
            ConsoleOutput output = new ConsoleOutput();

            output.WriteHeader();

            string expectedOutput = "NBehave version 0.3.0.0\r\nCopyright (C) 2007 Jimmy Bogard, Joe Ocampo, Morgan Persson, Tim Haughton.\r\nAll Rights Reserved.\r\n";

            Assert.That(_output.ToString(), Is.EqualTo(expectedOutput));
        }

        [Test]
        public void Should_output_environment_with_correct_runtime_environment_properties()
        {
            ConsoleOutput output = new ConsoleOutput();

            output.WriteRuntimeEnvironment();

            string osVersion = Environment.OSVersion.ToString();
            string clrVersion = Environment.Version.ToString();

            string expectedOutput = "Runtime Environment -\r\n   OS Version: " + osVersion + "\r\n  CLR Version: " + clrVersion + "\r\n";
            Debug.WriteLine(expectedOutput);


            Assert.That(_output.ToString(), Is.EqualTo(expectedOutput));
        }

        [Test]
        public void Should_output_passing_story_results()
        {
            ConsoleOutput output = new ConsoleOutput();
            StoryResults results = new StoryResults();

            results.AddResult(new ScenarioResults("Test", "Test", ScenarioResult.Passed));
            results.AddResult(new ScenarioResults("Test", "Test", ScenarioResult.Passed));
            results.AddResult(new ScenarioResults("Test", "Test", ScenarioResult.Passed));

            output.WriteDotResults(results);

            string expectedOutput = "...\r\n";

            Assert.That(_output.ToString(), Is.EqualTo(expectedOutput));
        }

        [Test]
        public void Should_output_failing_story_results()
        {
            ConsoleOutput output = new ConsoleOutput();
            StoryResults results = new StoryResults();

            results.AddResult(new ScenarioResults("Test", "Test", ScenarioResult.Failed));
            results.AddResult(new ScenarioResults("Test", "Test", ScenarioResult.Failed));
            results.AddResult(new ScenarioResults("Test", "Test", ScenarioResult.Failed));

            output.WriteDotResults(results);

            string expectedOutput = "FFF\r\n";

            Assert.That(_output.ToString(), Is.EqualTo(expectedOutput));
        }

        [Test]
        public void Should_output_pending_story_results()
        {
            ConsoleOutput output = new ConsoleOutput();
            StoryResults results = new StoryResults();

            results.AddResult(new ScenarioResults("Test", "Test", ScenarioResult.Pending));
            results.AddResult(new ScenarioResults("Test", "Test", ScenarioResult.Pending));
            results.AddResult(new ScenarioResults("Test", "Test", ScenarioResult.Pending));

            output.WriteDotResults(results);

            string expectedOutput = "PPP\r\n";

            Assert.That(_output.ToString(), Is.EqualTo(expectedOutput));
        }

        [Test]
        public void Should_output_complex_story_results()
        {
            ConsoleOutput output = new ConsoleOutput();
            StoryResults results = new StoryResults();

            results.AddResult(new ScenarioResults("Title", "Title", ScenarioResult.Passed));
            results.AddResult(new ScenarioResults("Title", "Title", ScenarioResult.Failed));
            results.AddResult(new ScenarioResults("Title", "Title", ScenarioResult.Pending));
            results.AddResult(new ScenarioResults("Title", "Title", ScenarioResult.Passed));
            results.AddResult(new ScenarioResults("Title", "Title", ScenarioResult.Failed));
            results.AddResult(new ScenarioResults("Title", "Title", ScenarioResult.Failed));

            output.WriteDotResults(results);

            string expectedOutput = ".FP.FF\r\n";

            Assert.That(_output.ToString(), Is.EqualTo(expectedOutput));
        }

        [Test]
        public void Should_output_totals_from_summary_results()
        {
            ConsoleOutput output = new ConsoleOutput();
            StoryResults results = new StoryResults();

            results.AddResult(new ScenarioResults("Title", "Title", ScenarioResult.Passed));
            results.AddResult(new ScenarioResults("Title", "Title", ScenarioResult.Failed));
            results.AddResult(new ScenarioResults("Title", "Title", ScenarioResult.Pending));
            results.AddResult(new ScenarioResults("Title", "Title", ScenarioResult.Passed));
            results.AddResult(new ScenarioResults("Title", "Title", ScenarioResult.Failed));
            results.AddResult(new ScenarioResults("Title", "Title", ScenarioResult.Failed));

            output.WriteSummaryResults(results);

            string expectedOutput = "Scenarios run: 6, Failures: 3, Pending: 1\r\n";

            Assert.That(_output.ToString(), Is.EqualTo(expectedOutput));
        }

        [Test]
        public void Should_output_failure_details_from_failure_output()
        {
            ConsoleOutput output = new ConsoleOutput();
            StoryResults results = new StoryResults();

            ScenarioResults scenarioResults = new ScenarioResults("Story", "Scenario", ScenarioResult.Passed);
            Exception error = new Exception("Failure");

            scenarioResults.Fail(error);

            results.AddResult(scenarioResults);
            output.WriteFailures(results);

            string expectedOutput = "\r\nFailures:\r\n1) Story (Scenario) FAILED\r\n  System.Exception : Failure\r\n\r\n";

            Assert.That(_output.ToString(), Is.EqualTo(expectedOutput));
        }

        [Test]
        public void Should_only_output_failed_scenarios_in_failure_output()
        {
            ConsoleOutput output = new ConsoleOutput();
            StoryResults results = new StoryResults();

            ScenarioResults scenarioResults = new ScenarioResults("Story", "Scenario", ScenarioResult.Passed);
            Exception error = new Exception("Failure");

            scenarioResults.Fail(error);

            results.AddResult(new ScenarioResults("Title", "Title", ScenarioResult.Passed));
            results.AddResult(new ScenarioResults("Title", "Title", ScenarioResult.Pending));
            results.AddResult(scenarioResults);
            output.WriteFailures(results);

            string expectedOutput = "\r\nFailures:\r\n1) Story (Scenario) FAILED\r\n  System.Exception : Failure\r\n\r\n";

            Assert.That(_output.ToString(), Is.EqualTo(expectedOutput));
        }

        [Test]
        public void Should_output_newline_from_separator()
        {
            ConsoleOutput output = new ConsoleOutput();

            output.WriteSeparator();

            Assert.That(_output.ToString(), Is.EqualTo(Environment.NewLine));
        }

        [Test]
        public void Should_output_pending_details_from_pending_output()
        {
            ConsoleOutput output = new ConsoleOutput();
            StoryResults results = new StoryResults();

            ScenarioResults scenarioResults = new ScenarioResults("Story", "Scenario", ScenarioResult.Passed);

            scenarioResults.Pend("reason");

            results.AddResult(scenarioResults);
            output.WritePending(results);

            string expectedOutput = "\r\nPending:\r\n1) Story (Scenario): reason\r\n";

            Assert.That(_output.ToString(), Is.EqualTo(expectedOutput));
        }

        [Test]
        public void Should_only_output_pending_scenarios_in_pending_output()
        {
            ConsoleOutput output = new ConsoleOutput();
            StoryResults results = new StoryResults();

            ScenarioResults scenarioResults = new ScenarioResults("Story", "Scenario", ScenarioResult.Passed);
            
            scenarioResults.Pend("reason");

            results.AddResult(new ScenarioResults("Title", "Title", ScenarioResult.Passed));
            results.AddResult(new ScenarioResults("Title", "Title", ScenarioResult.Failed));
            results.AddResult(scenarioResults);
            output.WritePending(results);

            string expectedOutput = "\r\nPending:\r\n1) Story (Scenario): reason\r\n";

            Assert.That(_output.ToString(), Is.EqualTo(expectedOutput));
        }


    }
}
