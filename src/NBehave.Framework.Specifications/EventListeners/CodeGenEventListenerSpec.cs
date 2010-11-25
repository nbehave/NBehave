using System.IO;
using System.Text;
using NBehave.Narrator.Framework.EventListeners;
using NBehave.Narrator.Framework.Specifications.Features;
using NUnit.Framework;

namespace NBehave.Narrator.Framework.Specifications.EventListeners
{
    [TestFixture]
    public class CodeGenEventListenerSpec
    {
        private string _output;

        [SetUp]
        public virtual void SetUp()
        {
            TextWriter output = new StringWriter(new StringBuilder());
            EventListener listener = new CodeGenEventListener(output);

            NBehaveConfiguration
                .New
                .SetEventListener(listener)
                .SetAssemblies(new[] { GetType().Assembly.Location })
                .SetScenarioFiles(new[] { TestFeatures.FeaturesAndScenarios })
                .Run();

            _output = output.ToString();
        }

        public class WhenRunningWithCodegen : CodeGenEventListenerSpec
        {
            [Test]
            public void ShouldGenerateCodeForStepGivenSomethingPending()
            {
                StringAssert.Contains(@"[Given(""something pending"")]", _output);
            }

            [Test]
            public void ShouldNotGenerateCodeForStepGivenSomethingPending()
            {
                StringAssert.DoesNotContain(@"[Given(""something"")]", _output);
            }

            [Test]
            public void ShouldGenerateCodeForStepPendingAndAsGiven()
            {
                StringAssert.DoesNotContain(@"[And(""", _output);
                StringAssert.Contains(@"[Given(""something more pending"")]", _output);
            }

            [Test]
            public void ShouldGenerateCodeForStepPendingAndAsWhen()
            {
                StringAssert.DoesNotContain(@"[And(""", _output);
                StringAssert.Contains(@"[When(""some more pending event occurs"")]", _output);
            }
        }
    }
}