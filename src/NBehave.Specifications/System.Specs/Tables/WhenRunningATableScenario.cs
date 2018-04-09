using System.Collections.Generic;
using System.IO;
using System.Linq;
using NBehave.Extensions;
using NUnit.Framework;

namespace NBehave.Specifications.System.Specs.Tables
{
    [TestFixture]
    public class WhenRunningATableScenario : SystemTestContext
    {
        private FeatureResults _results;

        protected override void EstablishContext()
        {
            _config = ConfigurationNoAppDomain
                .New
                .SetAssemblies(new[] { Path.GetFileName(GetType().Assembly.Location) })
                .SetEventListener(NBehave.EventListeners.EventListeners.NullEventListener())
                .SetScenarioFiles(new[] { Path.Combine("System.Specs", "Tables", "TableScenario.feature") });
        }

        protected override void Because()
        {
            _results = _config.Build().Run();
        }

        [Test]
        public void AllStepsShouldPass()
        {
            var stepResults = _results.SelectMany(_ => _.ScenarioResults).SelectMany(result => result.StepResults).ToList();
            var results = stepResults.Select(stepResult => stepResult.Result).ToList();

            Assert.That(results.Count, Is.Not.EqualTo(0));
            foreach (var result in results)
                Assert.That(result, Is.TypeOf(typeof (Passed)), result.Message);
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
