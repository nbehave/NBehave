using System.Collections.Generic;
using NBehave.Narrator.Framework;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using gen = System.Collections.Generic;

namespace NBehave.Narrator.Framework.Specifications
{
    [TestFixture]
    public class StoryResultsFixture
    {
        [Test]
        public void Should_report_scenario_totals_based_on_results_added()
        {
            StoryResults results = new StoryResults();

            results.AddResult(new ScenarioResults("Title", "Title", ScenarioResult.Passed));
            results.AddResult(new ScenarioResults("Title", "Title", ScenarioResult.Failed));
            results.AddResult(new ScenarioResults("Title", "Title", ScenarioResult.Pending));
            results.AddResult(new ScenarioResults("Title", "Title", ScenarioResult.Passed));
            results.AddResult(new ScenarioResults("Title", "Title", ScenarioResult.Failed));
            results.AddResult(new ScenarioResults("Title", "Title", ScenarioResult.Failed));

            Assert.That(results.NumberOfPassingScenarios, Is.EqualTo(2));
            Assert.That(results.NumberOfFailingScenarios, Is.EqualTo(3));
            Assert.That(results.NumberOfPendingScenarios, Is.EqualTo(1));
        }

        [Test]
        public void Should_return_scenario_results_added()
        {
            StoryResults results = new StoryResults();

            results.AddResult(new ScenarioResults("Passed", "Passed", ScenarioResult.Passed));
            results.AddResult(new ScenarioResults("Failed", "Failed", ScenarioResult.Failed));

            gen.List<ScenarioResults> scenarioResults = new List<ScenarioResults>(results.ScenarioResults);

            Assert.That(scenarioResults.Count, Is.EqualTo(2));
            Assert.That(scenarioResults[0].StoryTitle, Is.EqualTo("Passed"));
            Assert.That(scenarioResults[1].StoryTitle, Is.EqualTo("Failed"));
        }
    }
}