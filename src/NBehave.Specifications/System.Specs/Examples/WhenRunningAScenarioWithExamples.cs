using NBehave.Attributes;
using NBehave.Domain;
using NBehave.Extensions;
using NUnit.Framework;

namespace NBehave.Specifications.System.Specs.Examples
{
    [TestFixture]
    public class WhenRunningAScenarioWithScenarioOutlines : SystemTestContext
    {
        private FeatureResults _results;

        protected override void EstablishContext()
        {
            Configure_With(@"System.Specs\Examples\Examples.feature");
        }

        protected override void Because()
        {
            _results = _config.Build().Run();
        }

        [Test]
        public void AllStepsShouldPass()
        {
            Assert.That(_results.NumberOfPassingScenarios, Is.EqualTo(1));
        }
    }

    [ActionSteps]
    public class ScenarioOutlineSteps
    {
        [Given("this scenario containing examples $col1")]
        public void Given(int col1)
        {
        }

        [When("the scenario is executed $col2")]
        public void When(int col2)
        {
        }

        [Then("it should be templated and executed with each $row")]
        public void Then(int row)
        {
        }
    }
}