namespace NBehave.Narrator.Framework.Specifications.System.Specs
{
    using NBehave.Narrator.Framework.EventListeners;

    using NUnit.Framework;

    [TestFixture]
    public class WhenRunningAScenarioWithArguments : SystemTestContext
    {
        private NBehaveConfiguration _config;
        private FeatureResults _results;

        protected override void EstablishContext()
        {
            _config = NBehaveConfiguration
                .New
                .SetAssemblies(new[] { "NBehave.Narrator.Framework.Specifications.dll" })
                .SetEventListener(EventListeners.NullEventListener())
                .SetScenarioFiles(new[] { @"System.Specs\Scenarios\ScenarioWithArguments.feature" });
        }

        protected override void Because()
        {
            this._results = this._config.Run();
        }

        [Test]
        public void AllStepsShouldPass()
        {
            Assert.That(this._results.NumberOfPassingScenarios, Is.EqualTo(1)); 
        }
    }

    [ActionSteps]
    public class ScenarioStepsWithArguments
    {
        [Given("a scenario that has $arguments")]
        public void Given(string arguments)
        {
            Assert.That(arguments, Is.EqualTo("arguments"));
        }

        [When("the scenario is $executed")]
        public void When(string executed)
        {
            Assert.That(executed, Is.EqualTo("executed"));
        }

        [Then("it should $pass")]
        public void Then(string pass)
        {
            Assert.That(pass, Is.EqualTo("pass"));
        }
    }
}