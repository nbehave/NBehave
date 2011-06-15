namespace NBehave.Narrator.Framework.Specifications.System.Specs
{
    using global::System.Collections.Generic;
    using global::System.Linq;

    using NBehave.Narrator.Framework.EventListeners;

    using NUnit.Framework;

    [TestFixture]
    public class WhenRunningATableScenario : SystemTestContext
    {
        private NBehaveConfiguration _config;
        private FeatureResults _results;

        protected override void EstablishContext()
        {
            _config = NBehaveConfiguration
                .New
                .SetAssemblies(new[] { "NBehave.Narrator.Framework.Specifications.dll" })
                .SetEventListener(EventListeners.NullEventListener())
                .SetScenarioFiles(new[] { @"System.Specs\Tables\TableScenario.feature" });
        }

        protected override void Because()
        {
            this._results = this._config.Run();
        }

        [Test]
        public void AllStepsShouldPass()
        {
            IEnumerable<ActionStepResult> enumerable = _results.ScenarioResults.SelectMany(result => result.ActionStepResults);
            IEnumerable<Result> results = enumerable.Select(stepResult => stepResult.Result);

            foreach (var result in results)
            {
                Assert.That(result, Is.TypeOf(typeof(Passed)), result.Message);       
            }
        }
    }

    [ActionSteps]
    public class TableScenarioSteps
    {
        Dictionary<string,string> _rows = new Dictionary<string, string>();

        [Given("this tabled scenario:")]
        public void Given(string latin, string english)
        {
            this._rows.Add(latin, english);        
        }

        [When("this tabled scenario is executed")]
        public void When() { }

        [Then("this tabled scenario should pass:")]
        public void Then(string latin, string english)
        {
            Assert.That(_rows[latin], Is.EqualTo(english));
        }
    }
}