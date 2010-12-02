using System.Collections.Generic;
using System.IO;
using System.Text;
using NBehave.Narrator.Framework.EventListeners;
using NBehave.Narrator.Framework.Text;
using NUnit.Framework;

namespace NBehave.Narrator.Framework.Specifications.EventListeners
{
    [TestFixture]
    public class CodeGenEventListenerSpec
    {
        private string _feature;
        private string _output;

        [SetUp]
        public virtual void SetUp()
        {
            TextWriter output = new StringWriter(new StringBuilder());
            IEventListener listener = new CodeGenEventListener(output);
            var runner = new TextRunner(listener);
            runner.LoadAssembly(GetType().Assembly);
            runner.Load(_feature.ToStream());
            runner.Run();
            _output = output.ToString();
        }

        public class When_running_with_codegen : CodeGenEventListenerSpec
        {
            public override void SetUp()
            {
                _feature = XmlOutputEventListenerTestData.Feature;
                base.SetUp();
            }

            [Test]
            public void Should_generate_code_for_step_Given_something_pending()
            {
                StringAssert.Contains(@"[Given(""something pending"")]", _output);
            }

            [Test]
            public void Should_NOT_generate_code_for_step_Given_something_pending()
            {
                StringAssert.DoesNotContain(@"[Given(""something"")]", _output);
            }

            [Test]
            public void Should_generate_code_for_step_pending_And_as_Given()
            {
                StringAssert.DoesNotContain(@"[And(""", _output);
                StringAssert.Contains(@"[Given(""something more pending"")]", _output);
            }

            [Test]
            public void Should_generate_code_for_step_pending_And_as_When()
            {
                StringAssert.DoesNotContain(@"[And(""", _output);
                StringAssert.Contains(@"[When(""some more pending event occurs"")]", _output);
            }
        }
    }
}