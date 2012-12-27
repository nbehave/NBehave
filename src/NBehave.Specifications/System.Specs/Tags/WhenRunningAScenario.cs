using System.Linq;
using NBehave.Extensions;
using NUnit.Framework;

namespace NBehave.Specifications.System.Specs.Tags
{
    [TestFixture]
    public abstract class WhenRunningAScenarioWithTags : SystemTestContext
    {
        private FeatureResults _results;

        protected override void EstablishContext()
        {
            Configure_With(@"System.Specs\Tags\Tags.feature");
        }

        protected override void Because()
        {
            _results = _config.Build().Run();
        }

        public class When_running_only_scenario_with_tag : WhenRunningAScenarioWithTags
        {
            protected override void EstablishContext()
            {
                base.EstablishContext();
                _config.UseTagsFilter(new[] { new[] { "@Tag4" } }.ToList());
            }

            [Test]
            public void Should_run_scenario_1()
            {
                var scenarioResults = _results.SelectMany(_ => _.ScenarioResults);
                Assert.That(scenarioResults.Count(), Is.EqualTo(1));
                Assert.That(scenarioResults.First().ScenarioTitle, Is.EqualTo("Scenario1"));
            }

            [Test]
            public void Should_not_run_scenario_2()
            {
                var scenarioResults = _results.SelectMany(_ => _.ScenarioResults);
                Assert.That(scenarioResults.Any(_ => _.ScenarioTitle == "Scenario2"), Is.False);
            }
        }

        public class When_running_with_tag_that_matches_no_scenario : WhenRunningAScenarioWithTags
        {
            protected override void EstablishContext()
            {
                base.EstablishContext();
                _config.UseTagsFilter(new[] { new[] { "@Tag99" } }.ToList());
            }

            [Test]
            public void Should_not_run_any_scenario()
            {
                var scenarioResults = _results.SelectMany(_ => _.ScenarioResults);
                Assert.That(scenarioResults.Count(), Is.EqualTo(0));
            }
        }

        public class When_running_scenarios_excluding_tag : WhenRunningAScenarioWithTags
        {
            protected override void EstablishContext()
            {
                base.EstablishContext();
                _config.UseTagsFilter(new[] { new[] { "~@Tag4" } }.ToList());
            }

            [Test]
            public void Should_run_scenario_1()
            {
                var scenarioResults = _results.SelectMany(_ => _.ScenarioResults);
                Assert.That(scenarioResults.Count(), Is.EqualTo(1));
                Assert.That(scenarioResults.First().ScenarioTitle, Is.EqualTo("Scenario2"));
            }

            [Test]
            public void Should_not_run_scenario_2()
            {
                var scenarioResults = _results.SelectMany(_ => _.ScenarioResults);
                Assert.That(scenarioResults.Any(_ => _.ScenarioTitle == "Scenario1"), Is.False);
            }
        }

        public class When_running_scenarios_with_both_excluding_and_including_tags : WhenRunningAScenarioWithTags
        {
            protected override void EstablishContext()
            {
                base.EstablishContext();
                _config.UseTagsFilter(new[] { new[] { "@Tag2" }, new[] { "~@Tag4" } }.ToList());
            }

            [Test]
            public void Should_run_scenario_1()
            {
                var scenarioResults = _results.SelectMany(_ => _.ScenarioResults);
                Assert.That(scenarioResults.Count(), Is.EqualTo(1));
                Assert.That(scenarioResults.First().ScenarioTitle, Is.EqualTo("Scenario2"));
            }

            [Test]
            public void Should_not_run_scenario_2()
            {
                var scenarioResults = _results.SelectMany(_ => _.ScenarioResults);
                Assert.That(scenarioResults.Any(_ => _.ScenarioTitle == "Scenario1"), Is.False);
            }
        }
    }
}