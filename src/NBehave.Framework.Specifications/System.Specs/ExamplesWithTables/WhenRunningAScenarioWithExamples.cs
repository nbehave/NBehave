namespace NBehave.Narrator.Framework.Specifications.System.Specs
{
    using NBehave.Narrator.Framework.EventListeners;

    using NUnit.Framework;

    [TestFixture, Ignore]
    public class WhenRunningAScenarioWithScenarioOutlinesAndTables : SystemTestContext
    {
        private NBehaveConfiguration _config;
        private FeatureResults _results;

        protected override void EstablishContext()
        {
            _config = NBehaveConfiguration
                .New
                .SetAssemblies(new[] { "NBehave.Narrator.Framework.Specifications.dll" })
                .SetEventListener(EventListeners.NullEventListener())
                .SetScenarioFiles(new[] { @"System.Specs\ScenarioOutlinesWithTables\ScenarioOutlinesWithTables.feature" });
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
    public class ScenarioOutlineWithTableSteps
    {
        [Given("this scenario containing scenario outline and a table:")]
        public void Given(int left, int right){}

        [When("the tabled scenario outline is executed")]
        public void When(){}

        [Then("the table should be templated into the scenario outline and executed with each row:")]
        public void Then(int sum){}
    }
}