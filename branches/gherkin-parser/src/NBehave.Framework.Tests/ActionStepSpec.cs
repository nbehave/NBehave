using System.Collections.Generic;
using NUnit.Framework;

namespace NBehave.Narrator.Framework.Specifications
{
    [TestFixture]
    public class ActionStepSpec
    {
        private ActionStep _actionStep;

        [SetUp]
        public void Establish_context()
        {
            var lang = new List<Language>();
            Language en = BuildEnglish();
            lang.Add(en);
            _actionStep = new ActionStep(lang);
        }

        private Language BuildEnglish()
        {
            var enYmlEntry = new YmlEntry("en");
            var feature = new YmlEntry("feature");
            feature.AddValue("Feature");
            feature.AddValue("Story");
            enYmlEntry.Values.Add(feature);
            var steps = new Dictionary<string, string>
                            {
                                {"scenario", "Scenario"}, 
                                {"examples", "Examples"}, 
                                {"given", "Given"}, 
                                {"when", "When"},
                                {"then", "Then"},
                                {"and", "And"},
                                {"but", "But"},
                            };
            foreach (var step in steps)
            {
                var e = new YmlEntry(step.Key);
                e.AddValue(step.Value);
                enYmlEntry.Values.Add(e);
            }
            return new Language(enYmlEntry);
        }

        [Test]
        public void Should_pass_as_a_scenario_title()
        {
            Assert.That(_actionStep.IsScenarioTitle("Scenario A test scenario"), Is.True);
        }

        [Test]
        public void Should_not_be_a_scenario_title()
        {
            Assert.That(_actionStep.IsScenarioTitle("Foo: A test scenario"), Is.False);
        }

        [Test]
        public void Colon_should_be_allowed_after_scenario()
        {
            Assert.That(_actionStep.IsScenarioTitle("Scenario: A test scenario"), Is.True);
        }

        [Test]
        public void Should_Get_title_of_scenario()
        {
            Assert.That(_actionStep.GetTitle("Scenario: A test scenario"), Is.EqualTo("A test scenario"));
        }

        [Test]
        public void Should_pass_as_a_story_title()
        {
            Assert.That(_actionStep.IsFeatureTitle("Story A story"), Is.True);
        }

        [Test]
        public void Should_not_be_a_story_title()
        {
            Assert.That(_actionStep.IsFeatureTitle("Foo: A story"), Is.False);
        }

        [Test]
        public void Colon_should_be_allowed_after_story()
        {
            Assert.That(_actionStep.IsFeatureTitle("Story: A story"), Is.True);
        }

        [Test]
        public void Should_Get_title_of_story()
        {
            Assert.That(_actionStep.GetTitle("Story: A story"), Is.EqualTo("A story"));
        }

        [Test]
        public void Should_pass_as_scenario_step()
        {
            foreach (var step in _actionStep.ScenarioSteps)
            {
                Assert.That(_actionStep.IsScenarioStep(step + " X"), Is.True,
                    string.Format("'{0}' should be a scenario step", step));
            }
        }
    }
}
