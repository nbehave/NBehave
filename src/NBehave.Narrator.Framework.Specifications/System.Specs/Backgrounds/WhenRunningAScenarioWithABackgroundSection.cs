using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace NBehave.Narrator.Framework.Specifications.System.Specs
{
    [TestFixture]
    public class WhenRunningAScenarioWithABackgroundSection : SystemTestContext
    {
        private NBehaveConfiguration _config;
        private FeatureResults _results;

        protected override void EstablishContext()
        {
            _config = NBehaveConfiguration
                .New
                .SetAssemblies(new[] {"NBehave.Narrator.Framework.Specifications.dll"})
                .SetEventListener(Framework.EventListeners.EventListeners.NullEventListener())
                .SetScenarioFiles(new[] {@"System.Specs\Backgrounds\Background.feature"});
        }

        protected override void Because()
        {
            _results = _config.Build().Run();
        }

        [Test]
        public void AllStepsShouldPass()
        {
            IEnumerable<ActionStepResult> enumerable = _results.ScenarioResults.SelectMany(result => result.ActionStepResults);
            IEnumerable<Result> results = enumerable.Select(stepResult => stepResult.Result);

            foreach (var result in results)
            {
                Assert.That(result, Is.TypeOf(typeof (Passed)), result.Message);
            }
        }
    }

    [ActionSteps]
    public class ScenarioStepsForBackground
    {
        private int callCount;

        [Given("this background section declaration")]
        public void FirstGiven()
        {
            Assert.That(callCount, Is.EqualTo(0));
            callCount++;
        }

        [Given("this one")]
        public void SecondGiven()
        {
            Assert.That(callCount, Is.EqualTo(1));
            callCount++;
        }

        [Given("this scenario under the context of a background section")]
        public void ThirdGiven()
        {
            Assert.That(callCount, Is.EqualTo(2));
            callCount++;
        }

        [When("the scenario with a background section is executed")]
        public void When()
        {
            Assert.That(callCount, Is.EqualTo(3));
            callCount++;
        }

        [Then("the background section steps should be called before this scenario")]
        public void AnotherThen()
        {
            Assert.That(callCount, Is.EqualTo(4));
            callCount++;
        }
    }
}