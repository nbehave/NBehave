using NBehave.Attributes;
using NBehave.Domain;
using NBehave.Extensions;
using NUnit.Framework;
using Should.Fluent;

namespace NBehave.Specifications.System.Specs.Scenarios
{
    [TestFixture]
    public class WhenRunningAScenarioWithAFailingStep : SystemTestContext
    {
        private FeatureResults _results;

        protected override void EstablishContext()
        {
           Configure_With(@"System.Specs\Scenarios\ScenarioWithFailingStep.feature");
        }

        protected override void Because()
        {
            _results = _config.Build().Run();
        }

        [Test]
        public void ShouldDisplayErrorMessageForFailingStep()
        {
            Assert.That(_results.NumberOfFailingScenarios, Is.EqualTo(1));
            StringAssert.StartsWith("Should.Core.Exceptions.EqualException", _results[0].ScenarioResults[0].Message);
        }
    }

    [ActionSteps]
    public class ScenarioStepsWithFailingStep
    {
        [Given("this failing scenario")]
        public void Given()
        {
        }

        [When("this failing scenario is executed")]
        public void When()
        {
        }

        [Then("the failing scenario should display an error message")]
        public void Then()
        {
            1.Should().Equal(2);
        }
    }
}