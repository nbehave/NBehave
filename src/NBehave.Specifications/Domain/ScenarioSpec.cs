using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace NBehave.Narrator.Framework.Specifications.Domain
{
    [TestFixture]
    public class ScenarioSpec
    {
        [Test]
        public void Should_convert_scenario_to_string()
        {
            var feature = new Feature("featureTitle", "", "source", 1);
            var scenario = new Scenario("scenarioTitle", "source", feature, 3);
            scenario.AddStep(new StringStep("Given a step", "source, 4"));
            scenario.AddStep(new StringStep("And another step", "source, 5"));
            var scenarioAsString = scenario.ToString();
            string expected = string.Format("Scenario: scenarioTitle{0}  Given a step{0}  And another step", Environment.NewLine);
            Assert.AreEqual(expected, scenarioAsString);
        }

        [Test]
        public void Should_include_examples_when_converting_to_string()
        {
            var feature = new Feature("featureTitle", "", "source", 1);
            var scenario = new Scenario("scenarioTitle", "source", feature, 3);
            scenario.AddStep(new StringStep("Given a [foo]", "source, 4"));
            scenario.AddStep(new StringStep("And [bar]", "source, 5"));
            var columnNames = new ExampleColumns { new ExampleColumn("foo"), new ExampleColumn("bar") };
            scenario.AddExamples(new[]
                                     {
                                       new Example(columnNames, new Dictionary<string, string>{ {"foo", "1"}, {"bar", "2"}}),
                                       new Example(columnNames, new Dictionary<string, string>{ {"foo", "11"}, {"bar", "22"}})
                                     });
            var scenarioAsString = scenario.ToString();
            string expected = string.Format("Scenario: scenarioTitle{0}  Given a [foo]{0}  And [bar]{0}", Environment.NewLine);
            expected += "Examples:" + Environment.NewLine +
                        "     | foo | bar |" + Environment.NewLine +
                        "     | 1 | 2 |" + Environment.NewLine +
                        "     | 11 | 22 |";
            Assert.AreEqual(expected, scenarioAsString);
        }
    }
}