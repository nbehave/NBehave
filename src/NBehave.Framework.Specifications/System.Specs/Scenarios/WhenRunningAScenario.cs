namespace NBehave.Narrator.Framework.Specifications.System.Specs
{
    using NBehave.Narrator.Framework.EventListeners;

    using NUnit.Framework;

    [TestFixture]
    public class WhenRunningAScenario : SystemTestContext
    {
        private NBehaveConfiguration _config;
        private FeatureResults _results;

        protected override void EstablishContext()
        {
            _config = NBehaveConfiguration
                .New
                .SetAssemblies(new[] { "NBehave.Narrator.Framework.Specifications.dll" })
                .SetEventListener(EventListeners.NullEventListener())
                .SetScenarioFiles(new[] { @"System.Specs\Scenarios\Scenario.feature" });
        }

        protected override void Because()
        {
            this._results = this._config.Run();
        }

        [Test]
        public void AllStepsShouldPass()
        {
            Assert.That(this._results.NumberOfPassingScenarios, Is.EqualTo(2)); 
        }
    }

    [ActionSteps]
    public class ScenarioSteps
    {
        [Given("this scenario")]
        public void Given(){}

        [Given("another scenario")]
        public void AnotherGiven(){}

        [When("the scenario is executed")]
        public void When(){}

        [Then("it should pass")]
        public void Then(){}
        
        [Then("it should also pass")]
        public void AnotherThen(){}
    }
}