using NBehave.Extensions;
using NUnit.Framework;

namespace NBehave.Specifications.System.Specs.Scenarios
{
    [TestFixture]
    public class WhenRunningAScenarioWithoutAFeature : SystemTestContext
    {
        private FeatureResults result;

        protected override void EstablishContext()
        {
            Configure_With(@"System.Specs\Scenarios\ScenarioWithoutFeature.feature");
        }

        protected override void Because()
        {
            result = _config.Build().Run();
        }

        [Test]
        public void AllStepsShouldPass()
        {
            Assert.That(result.NumberOfPassingScenarios, Is.GreaterThan(0));
            Assert.That(result.NumberOfFailingScenarios, Is.EqualTo(0));
            Assert.That(result.NumberOfPendingScenarios, Is.EqualTo(0));
        }
    }

    [ActionSteps]
    public class ScenarioStepsWithoutFeature
    {
        [Given("this featureless scenario")]
        public void Given()
        {
        }

        [When("this featureless scenario is executed")]
        public void When()
        {
        }

        [Then("the user should be notified that this is not valid")]
        public void Then()
        {
        }
    }
}