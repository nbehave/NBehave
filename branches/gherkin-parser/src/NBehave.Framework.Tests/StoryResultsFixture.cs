//using System.Collections.Generic;
//using NBehave.Narrator.Framework;
//using NUnit.Framework;
//using NUnit.Framework.SyntaxHelpers;
//using gen = System.Collections.Generic;

//namespace NBehave.Narrator.Framework.Specifications
//{
//    [TestFixture]
//    public class StoryResultsFixture
//    {
//        [Test]
//        public void Should_report_scenario_totals_based_on_results_added()
//        {
//            var results = new FeatureResults();

//            results.AddResult(new ScenarioResult("Title", "Title", Result.Passed));
//            results.AddResult(new ScenarioResult("Title", "Title", Result.Failed));
//            results.AddResult(new ScenarioResult("Title", "Title", Result.Pending));
//            results.AddResult(new ScenarioResult("Title", "Title", Result.Passed));
//            results.AddResult(new ScenarioResult("Title", "Title", Result.Failed));
//            results.AddResult(new ScenarioResult("Title", "Title", Result.Failed));

//            Assert.That(results.NumberOfPassingScenarios, Is.EqualTo(2));
//            Assert.That(results.NumberOfFailingScenarios, Is.EqualTo(3));
//            Assert.That(results.NumberOfPendingScenarios, Is.EqualTo(1));
//        }

//        [Test]
//        public void Should_return_scenario_results_added()
//        {
//            var results = new FeatureResults();

//            results.AddResult(new ScenarioResult("Passed", "Passed", Result.Passed));
//            results.AddResult(new ScenarioResult("Failed", "Failed", Result.Failed));

//            var scenarioResults = new List<ScenarioResult>(results.ScenarioResults);

//            Assert.That(scenarioResults.Count, Is.EqualTo(2));
//            Assert.That(scenarioResults[0].Story, Is.EqualTo("Passed"));
//            Assert.That(scenarioResults[1].Story, Is.EqualTo("Failed"));
//        }
//    }
//}