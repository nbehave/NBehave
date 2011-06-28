using System;
using System.IO;
using NBehave.Narrator.Framework.EventListeners;
using NBehave.Narrator.Framework.Specifications.Features;
using NUnit.Framework;

namespace NBehave.Narrator.Framework.Specifications.EventListeners
{
    [TestFixture]
    public class ColorfulConsoleOutputEventListenerSpec
    {
        private readonly string[] _feature = new[] {TestFeatures.ScenarioWithExamples};
        private string _output;
        private TextWriter _originalConsoleOut;

        [SetUp]
        public virtual void Setup()
        {
            var output = new MemoryStream();
            var writer = new StreamWriter(output);
            _originalConsoleOut = Console.Out;
            Console.SetOut(writer);
            var listener = new ColorfulConsoleOutputEventListener();
            NBehaveConfiguration
                .New
                .SetScenarioFiles(_feature)
                .SetAssemblies(new[] {GetType().Assembly.Location})
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
    }
}