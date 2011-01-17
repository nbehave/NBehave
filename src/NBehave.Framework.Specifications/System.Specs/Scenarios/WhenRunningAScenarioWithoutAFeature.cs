namespace NBehave.Narrator.Framework.Specifications.System.Specs
{
    using global::System.Collections.Generic;
    using global::System.Linq;

    using NBehave.Narrator.Framework.EventListeners;
    using NBehave.Narrator.Framework.Processors;

    using NUnit.Framework;

    [TestFixture]
    public class WhenRunningAScenarioWithoutAFeature : SystemTestContext
    {
        private NBehaveConfiguration _config;
        private FeatureResults _results;
        private ScenarioMustHaveFeatureException _exception;

        protected override void EstablishContext()
        {
            _config = NBehaveConfiguration
                .New
                .SetAssemblies(new[] { "NBehave.Narrator.Framework.Specifications.dll" })
                .SetEventListener(EventListeners.NullEventListener())
                .SetScenarioFiles(new[] { @"System.Specs\Scenarios\ScenarioWithoutFeature.feature" });
        }

        protected override void Because()
        {
            try
            {
                this._results = this._config.Run();
            }
            catch (ScenarioMustHaveFeatureException exception)
            {
                this._exception = exception;
            }
        }

        [Test]
        public void AllStepsShouldPass()
        {
            Assert.That(this._exception, Is.Not.Null);
        }
    }

    [ActionSteps]
    public class ScenarioStepsWithoutFeature
    {
        [Given("this featureless scenario")]
        public void Given(){}

        [When("this featureless scenario is executed")]
        public void When() { }

        [Then("the user should be notified that this is not valid")]
        public void Then(){}
    }
}