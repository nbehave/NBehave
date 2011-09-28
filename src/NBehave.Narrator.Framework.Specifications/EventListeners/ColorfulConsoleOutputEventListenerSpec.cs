using System;
using System.IO;
using NBehave.Narrator.Framework.Specifications.Features;
using NUnit.Framework;
using TestPlainTextAssembly;

namespace NBehave.Narrator.Framework.Specifications.EventListeners
{
    [TestFixture]
    public abstract class ColorfulConsoleOutputEventListenerSpec
    {
        private string _output;
        private TextWriter _originalConsoleOut;
        
        protected abstract string FeatureFile { get; }

        [SetUp]
        public void Setup()
        {
            var output = new MemoryStream();
            var writer = new StreamWriter(output);
            _originalConsoleOut = Console.Out;
            Console.SetOut(writer);
            var listener = Framework.EventListeners.EventListeners.ColorfulConsoleOutputEventListener();
            ConfigurationNoAppDomain
                .New
                .SetScenarioFiles(new[] { FeatureFile })
                .SetAssemblies(new[] { typeof(GreetingSystemActionSteps).Assembly.Location, GetType().Assembly.Location })
                .SetEventListener(listener)
                .Build()
                .Run();
            writer.Flush();
            output.Seek(0, SeekOrigin.Begin);
            var s = new StreamReader(output);
            _output = s.ReadToEnd();
        }

        [TearDown]
        public void Cleanup()
        {
            Console.SetOut(_originalConsoleOut);
        }

        public class When_running_a_feature_file_with_one_passing_scenario : ColorfulConsoleOutputEventListenerSpec
        {
            protected override string FeatureFile { get { return TestFeatures.FeatureNamedStory; } }

            [Test]
            public void Should_write_summary_of_steps()
            {
                StringAssert.Contains("Steps 3, failed 0, pending 0", _output);
            }

            [Test]
            public void Should_write_summary_of_Scenarios()
            {
                StringAssert.Contains("Scenarios run: 1, Failures: 0, Pending: 0", _output);
            }
        }

        public class When_running_scenario_with_tables : ColorfulConsoleOutputEventListenerSpec
        {
            protected override string FeatureFile { get { return TestFeatures.ScenarioWithExamples; } }

            [Test]
            public void Should_format_table_header_after_largest_column()
            {
                StringAssert.Contains("\t| str | strOut |", _output);
            }

            [Test]
            public void Should_format_table_row_after_largest_column()
            {
                StringAssert.Contains("\t| xyz | xyz    |", _output);
            }

            [Test]
            public void Should_write_summary_of_steps()
            {
                StringAssert.Contains("Steps 3, failed 0, pending 0", _output);
            }

            [Test]
            public void Should_write_summary_of_Scenarios()
            {
                StringAssert.Contains("Scenarios run: 1, Failures: 0, Pending: 0", _output);
            }            
        }

        public class When_running_scenario_with_unimplemented_step : ColorfulConsoleOutputEventListenerSpec
        {
            protected override string FeatureFile { get { return TestFeatures.FeatureWithPendingStep; } }

            [Test]
            public void Should_write_pending_if_step_result_is_PendingNotImplemented()
            {
                StringAssert.DoesNotContain(" - PENDINGNOTIMPLEMENTED", _output);
                StringAssert.Contains(" - PENDING", _output);
            }

            [Test]
            public void Should_write_summary_of_steps()
            {
                StringAssert.Contains("Steps 3, failed 0, pending 3", _output);
            }

            [Test]
            public void Should_write_summary_of_Scenarios()
            {
                StringAssert.Contains("Scenarios run: 1, Failures: 0, Pending: 1", _output);
            }
        }
    }
}