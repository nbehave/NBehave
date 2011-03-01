using Should.Fluent;
using Should.Fluent.Model;

namespace NBehave.Narrator.Framework.Specifications.System.Specs
{
    using global::System.Collections.Generic;
    using global::System.Linq;

    using NBehave.Narrator.Framework.EventListeners;

    using NUnit.Framework;

    [TestFixture]
    public class WhenRunningAScenarioWithAFailingStep : SystemTestContext
    {
        private NBehaveConfiguration _config;
        private FeatureResults _results;

        protected override void EstablishContext()
        {
            _config = NBehaveConfiguration
                .New
                .SetAssemblies(new[] { "NBehave.Narrator.Framework.Specifications.dll" })
                .SetEventListener(EventListeners.NullEventListener())
                .SetScenarioFiles(new[] { @"System.Specs\Scenarios\ScenarioWithFailingStep.feature" });
        }

        protected override void Because()
        {
            this._results = this._config.Run();
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
        public void Given() { }

        [When("this failing scenario is executed")]
        public void When() { }

        [Then("the failing scenario should display an error message")]
        public void Then()
        {
            1.Should().Equal(2);
        }
    }
}