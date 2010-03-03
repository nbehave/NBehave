using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;
using Context = NUnit.Framework.TestFixtureAttribute;

namespace NBehave.Narrator.Framework.Specifications.Text
{
    [Context]
    public abstract class ScenarioParserSpec
    {
        private readonly StringStepRunner _stringStepRunner = new StringStepRunner(new ActionCatalog());

        private ScenarioParser CreateScenarioParser()
        {
            return new ScenarioParser(_stringStepRunner);
        }

        private StringStep NewStringStep(string step)
        {
            return new StringStep(step, "filename", _stringStepRunner);
        }

        private List<ScenarioWithSteps> _scenarios;

        [Test]
        public virtual void Should_have_given_step()
        {
            CollectionAssert.Contains(_scenarios[0].Steps, NewStringStep("  Given numbers 1 and 2"));
        }

        [Test]
        public virtual void Should_have_when_step()
        {
            CollectionAssert.Contains(_scenarios[0].Steps, NewStringStep("  When I add the numbers"));
        }

        [Test]
        public virtual void Should_have_then_step()
        {
            CollectionAssert.Contains(_scenarios[0].Steps, NewStringStep("  Then the sum is 3"));
        }

        [TestFixture]
        public class Scenario_simple_scenario_without_title : ScenarioParserSpec
        {
            [SetUp]
            public void Scenario()
            {
                string scenario = "  Given numbers 1 and 2" + Environment.NewLine +
                                  "  When I add the numbers" + Environment.NewLine +
                                  "  Then the sum is 3";

                var parser = CreateScenarioParser();
                _scenarios = parser.Parse(scenario.ToStream());
            }
        }

        public class Scenario_simple_scenario_with_title : ScenarioParserSpec
        {
            [SetUp]
            public void Scenario()
            {
                string scenario = "Scenario: Adding numbers" + Environment.NewLine +
                                  "  Given numbers 1 and 2" + Environment.NewLine +
                                  "  When I add the numbers" + Environment.NewLine +
                                  "  Then the sum is 3";

                var parser = CreateScenarioParser();
                _scenarios = parser.Parse(scenario.ToStream());
            }

            [Test]
            public void should_find_3_steps()
            {
                Assert.That(_scenarios[0].Steps.Count(), Is.EqualTo(3));
            }

            [Test]
            public void Should_have_a_scenario_Title()
            {
                Assert.That(_scenarios[0].Title, Is.EqualTo("Adding numbers"));
            }
        }

        public class Scenario_two_scenarios_with_title : ScenarioParserSpec
        {
            [SetUp]
            public void Scenario()
            {
                string scenario = "Scenario: Adding numbers" + Environment.NewLine +
                                  "  Given numbers 1 and 2" + Environment.NewLine +
                                  "  When I add the numbers" + Environment.NewLine +
                                  "  Then the sum is 3" + Environment.NewLine +
                                  Environment.NewLine +
                                  "Scenario: Adding numbers again" + Environment.NewLine +
                                  "  Given numbers 3 and 5" + Environment.NewLine +
                                  "  When I add the numbers" + Environment.NewLine +
                                  "  Then the sum is 8";

                var parser = CreateScenarioParser();
                _scenarios = parser.Parse(scenario.ToStream());
            }

            [Test]
            public void should_find_2_scenarios()
            {
                Assert.That(_scenarios.Count(), Is.EqualTo(2));
            }

            [Test]
            public void Should_have_a_scenario_Title_on_both_scenarios()
            {
                Assert.That(_scenarios[0].Title, Is.EqualTo("Adding numbers"));
                Assert.That(_scenarios[1].Title, Is.EqualTo("Adding numbers again"));
            }
        }

        public class Scenario_Feature_with_scenario : ScenarioParserSpec
        {
            [SetUp]
            public void Scenario()
            {
                string scenario = "Feature: Calculator" + Environment.NewLine +
                                  "Scenario: Adding numbers" + Environment.NewLine +
                                  "  Given numbers 1 and 2" + Environment.NewLine +
                                  "  When I add the numbers" + Environment.NewLine +
                                  "  Then the sum is 3" + Environment.NewLine +
                                  Environment.NewLine +
                                  "Scenario: Adding numbers again" + Environment.NewLine +
                                  "  Given numbers 3 and 5" + Environment.NewLine +
                                  "  When I add the numbers" + Environment.NewLine +
                                  "  Then the sum is 8";

                var parser = CreateScenarioParser();
                _scenarios = parser.Parse(scenario.ToStream());
            }

            [Test]
            public void should_find_2_scenarios()
            {
                Assert.That(_scenarios.Count(), Is.EqualTo(2));
            }

            [Test]
            public void Should_have_a_feature_Title()
            {
                Assert.That(_scenarios[0].Feature.Title, Is.EqualTo("Calculator"));
            }
        }

        public class Scenario_Feature_narrative_is_all_text_upto_next_step_keyWord : ScenarioParserSpec
        {
            [SetUp]
            public void Scenario()
            {
                string scenario = "Feature: Calculator" + Environment.NewLine +
                                  "  This is the narrative" + Environment.NewLine +
                                  "  This is second row of narrative" + Environment.NewLine +
                                  "Scenario: Adding numbers" + Environment.NewLine +
                                  "  Given numbers 1 and 2" + Environment.NewLine +
                                  "  When I add the numbers" + Environment.NewLine +
                                  "  Then the sum is 3" + Environment.NewLine +
                                  Environment.NewLine +
                                  "Scenario: Adding numbers again" + Environment.NewLine +
                                  "  Given numbers 3 and 5" + Environment.NewLine +
                                  "  When I add the numbers" + Environment.NewLine +
                                  "  Then the sum is 8";

                var parser = CreateScenarioParser();
                _scenarios = parser.Parse(scenario.ToStream());
            }

            [Test]
            public void Should_have_narrative()
            {
                Assert.That(_scenarios[0].Feature.Narrative, Is.EqualTo(
                                                                 "  This is the narrative" + Environment.NewLine +
                                                                 "  This is second row of narrative"));
            }

            [Test]
            public void should_find_2_scenarios()
            {
                Assert.That(_scenarios.Count(), Is.EqualTo(2));
            }

            [Test]
            public void Should_have_a_feature_Title()
            {
                Assert.That(_scenarios[0].Feature.Title, Is.EqualTo("Calculator"));
            }
        }

        public class Scenario_scenario_with_example_table : ScenarioParserSpec
        {
            [SetUp]
            public void Scenario()
            {
                string scenario = "Scenario: Adding numbers" + Environment.NewLine +
                                  "  Given numbers [left] and [right]" + Environment.NewLine +
                                  "  When I add the numbers" + Environment.NewLine +
                                  "  Then the sum is [sum]" + Environment.NewLine +
                                  Environment.NewLine +
                                  "Examples:" + Environment.NewLine +
                                  "|left|right|sum|" + Environment.NewLine +
                                  "|1|2|3|" + Environment.NewLine +
                                  "|2|3|5";

                var parser = CreateScenarioParser();
                _scenarios = parser.Parse(scenario.ToStream());
            }

            [Test]
            public void Scenario_should_have_two_examples()
            {
                Assert.That(_scenarios[0].Examples.Count(), Is.EqualTo(2));
            }

            [Test]
            public void should_find_3_steps()
            {
                Assert.That(_scenarios[0].Steps.Count(), Is.EqualTo(3));
            }

            [Test]
            public void Should_have_a_scenario_Title()
            {
                Assert.That(_scenarios[0].Title, Is.EqualTo("Adding numbers"));
            }

            [Test]
            public override void Should_have_given_step()
            {
                CollectionAssert.Contains(_scenarios[0].Steps, NewStringStep("  Given numbers [left] and [right]"));
            }

            [Test]
            public override void Should_have_then_step()
            {
                CollectionAssert.Contains(_scenarios[0].Steps, NewStringStep("  Then the sum is [sum]"));
            }
        }

        public class Scenario_scenario_with_table_on_given : ScenarioParserSpec
        {
            private StringTableStep _step;

            [SetUp]
            public void Scenario()
            {
                string scenario = "  Given the following people exists:" + Environment.NewLine +
                                  "  |Name          |Country|" + Environment.NewLine +
                                  "  |Morgan Persson|Sweden |" + Environment.NewLine +
                                  "  |Jimmy Nilsson |Sweden |" + Environment.NewLine +
                                  "  |Jimmy bogard  |USA    |" + Environment.NewLine +
                                  "  When I search for people in sweden" + Environment.NewLine +
                                  "  Then I should get:" + Environment.NewLine +
                                  "  |Name          |" + Environment.NewLine +
                                  "  |Morgan Persson|" + Environment.NewLine +
                                  "  |Jimmy Nilsson |";

                var parser = CreateScenarioParser();
                _scenarios = parser.Parse(scenario.ToStream());
                _step = _scenarios[0].Steps.First() as StringTableStep;
            }

            [Test]
            public void given_step_should_have_three_table_steps()
            {
                Assert.That(_step, Is.Not.Null);
                Assert.That(_step.TableSteps.Count(), Is.EqualTo(3));
            }

            [Test]
            public void table_step_column_names_should_be_stored_in_lower_case()
            {
                var step = _step.TableSteps.First();
                CollectionAssert.Contains(step.ColumnNames, "name");
                Assert.That(step.ColumnValues["name"], Is.Not.Null); 
            }

            [Test]
            public override void Should_have_given_step()
            {
                CollectionAssert.Contains(_scenarios[0].Steps, NewStringStep("  Given the following people exists:"));
            }

            [Test]
            public override void Should_have_when_step()
            {
                CollectionAssert.Contains(_scenarios[0].Steps, NewStringStep("  When I search for people in sweden"));
            }

            [Test]
            public override void Should_have_then_step()
            {
                CollectionAssert.Contains(_scenarios[0].Steps, NewStringStep("  Then I should get:"));
            }
        }

        public class Scenario_Multiple_features : ScenarioParserSpec
        {
            [SetUp]
            public void Scenario()
            {
                string scenario = "Feature: Calculator 1" + Environment.NewLine +
                                  "Scenario: Adding numbers 1" + Environment.NewLine +
                                  "  Given numbers 1 and 2" + Environment.NewLine +
                                  "  When I add the numbers" + Environment.NewLine +
                                  "  Then the sum is 3" + Environment.NewLine +
                                  "" + Environment.NewLine +
                                  "Feature: Calculator 2" + Environment.NewLine +
                                  "Scenario: Adding numbers 2" + Environment.NewLine +
                                  "  Given numbers 1 and 2" + Environment.NewLine +
                                  "  When I add the numbers" + Environment.NewLine +
                                  "  Then the sum is 3";

                var parser = CreateScenarioParser();
                _scenarios = parser.Parse(scenario.ToStream());
            }

            [Test]
            public void feature_1_should_be_referenced_by_scenario_2()
            {
                Assert.That(_scenarios[0].Feature.Title, Is.EqualTo("Calculator 1"));
            }

            [Test]
            public void feature_2_should_be_referenced_by_scenario_2()
            {
                Assert.That(_scenarios[1].Feature.Title, Is.EqualTo("Calculator 2"));
            }
        }
    }
}