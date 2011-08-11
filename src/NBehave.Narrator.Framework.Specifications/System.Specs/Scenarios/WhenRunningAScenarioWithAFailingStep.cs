using System.IO;
using NUnit.Framework;
using Should.Fluent;

namespace NBehave.Narrator.Framework.Specifications.System.Specs
{
    [TestFixture]
    public class WhenRunningAScenarioWithAFailingStep : SystemTestContext
    {
        private NBehaveConfiguration _config;
        private FeatureResults _results;

        protected override void EstablishContext()
        {
            _config = ConfigurationNoAppDomain
                .New
                .SetAssemblies(new[] {Path.GetFileName(GetType().Assembly.Location)})
                .SetEventListener(Framework.EventListeners.EventListeners.NullEventListener())
                .SetScenarioFiles(new[] {@"System.Specs\Scenarios\ScenarioWithFailingStep.feature"});
        }

        protected override void Because()
        {
            _results = _config.Build().Run();
        }

        [Test]
        public void ShouldDisplayErrorMessageForFailingStep()
        {
            Assert.That(_results.NumberOfFailingScenarios, Is.EqualTo(1));
            Assert.That(_results.ScenarioResults[0].Message.StartsWith("Should.Core.Exceptions.EqualException"), Is.True);
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