namespace NBehave.Narrator.Framework.Specifications.System.Specs
{
    using global::System.Collections.Generic;
    using global::System.Linq;

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
            IEnumerable<ActionStepResult> enumerable = this._results.ScenarioResults.SelectMany(result => result.ActionStepResults);
            IEnumerable<Result> results = enumerable.Select(stepResult => stepResult.Result);

            foreach (var result in results)
            {
                Assert.That(result, Is.TypeOf(typeof(Passed)), result.Message);       
            }
        }
    }

    [ActionSteps]
    public class ScenarioSteps
    {
        [Given("this plain scenario")]
        public void Given(){}

        [Given("this second scenario")]
        public void AnotherGiven(){}

        [When("this plain scenario is executed")]
        public void When() { }

        [When("the second scenario is executed")]
        public void SecondWhen(){}

        [Then("this plain scenario should pass")]
        public void Then(){}

        [Then("it should also pass")]
        public void AnotherThen(){}
    }
}