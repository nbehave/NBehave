using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;

namespace NBehave.Narrator.Framework.Specifications.System.Specs
{
    [TestFixture]
    public class WhenRunningATableScenario : SystemTestContext
    {
        private NBehaveConfiguration _config;
        private FeatureResults _results;

        protected override void EstablishContext()
        {
            _config = NBehaveConfiguration
                .New
                .SetAssemblies(new[] {Path.GetFileName(GetType().Assembly.Location)})
                .SetEventListener(Framework.EventListeners.EventListeners.NullEventListener())
                .SetScenarioFiles(new[] {@"System.Specs\Tables\TableScenario.feature"});
        }

        protected override void Because()
        {
            _results = _config.Build().Run();
        }

        [Test]
        public void AllStepsShouldPass()
        {
            IEnumerable<StepResult> enumerable = _results.ScenarioResults.SelectMany(result => result.StepResults);
            IEnumerable<Result> results = enumerable.Select(stepResult => stepResult.Result);

            foreach (var result in results)
            {
                Assert.That(result, Is.TypeOf(typeof (Passed)), result.Message);
            }
        }
    }

    [ActionSteps]
    public class TableScenarioSteps
    {
        private readonly Dictionary<string, string> _rows = new Dictionary<string, string>();

        [Given("this tabled scenario:")]
        public void Given(string latin, string english)
        {
            _rows.Add(latin, english);
        }

        [When("this tabled scenario is executed")]
        public void When()
        {
        }

        [Then("this tabled scenario should pass:")]
        public void Then(string latin, string english)
        {
            Assert.That(_rows[latin], Is.EqualTo(english));
        }
    }
}