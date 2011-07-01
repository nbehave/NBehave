using System.IO;
using System.Linq;
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
        private CodeGenEventListener _listener;

        [SetUp]
        public virtual void SetUp()
        {
            _listener = new CodeGenEventListener();

            NBehaveConfiguration
                .New
                .SetEventListener(_listener)
                .SetAssemblies(new[] { GetType().Assembly.Location })
                .SetScenarioFiles(new[] { TestFeatures.FeaturesAndScenarios })
                .Build()
                .Run();

            _output = _listener.ToString();
        }

        public class WhenRunningWithCodegen : CodeGenEventListenerSpec
        {
            [Test]
            public void Should_put_each_pending_step_in_dictionary()
            {
                Assert.Greater(_listener.PendingSteps.Count(), 2);
                var step = _listener.PendingSteps.FirstOrDefault(_ => _.Step == "Given something pending");
                Assert.IsNotNull(step);
                Assert.AreEqual("S1", step.Feature);
                Assert.AreEqual("Pending scenario", step.Scenario);
            }

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