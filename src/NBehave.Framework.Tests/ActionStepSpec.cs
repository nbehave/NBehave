using System.Collections.Generic;
using NUnit.Framework;

namespace NBehave.Narrator.Framework.Specifications
{
    [TestFixture]
    public class ActionStepSpec
    {
        private ActionStep _actionStep;

        [SetUp]
        public void EstablishContext()
        {
            var lang = new List<Language>();
            var en = BuildEnglish();
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
        public void ShouldPassAsAScenarioTitle()
        {
            Assert.That(_actionStep.IsScenarioTitle("Scenario A test scenario"), Is.True);
        }

        [Test]
        public void ShouldNotBeAScenarioTitle()
        {
            Assert.That(_actionStep.IsScenarioTitle("Foo: A test scenario"), Is.False);
        }

        [Test]
        public void ColonShouldBeAllowedAfterScenario()
        {
            Assert.That(_actionStep.IsScenarioTitle("Scenario: A test scenario"), Is.True);
        }

        [Test]
        public void ShouldGetTitleOfScenario()
        {
            Assert.That(_actionStep.GetTitle("Scenario: A test scenario"), Is.EqualTo("A test scenario"));
        }

        [Test]
        public void ShouldPassAsAStoryTitle()
        {
            Assert.That(_actionStep.IsFeatureTitle("Story A story"), Is.True);
        }

        [Test]
        public void ShouldNotBeAStoryTitle()
        {
            Assert.That(_actionStep.IsFeatureTitle("Foo: A story"), Is.False);
        }

        [Test]
        public void ColonShouldBeAllowedAfterStory()
        {
            Assert.That(_actionStep.IsFeatureTitle("Story: A story"), Is.True);
        }

        [Test]
        public void ShouldGetTitleOfStory()
        {
            Assert.That(_actionStep.GetTitle("Story: A story"), Is.EqualTo("A story"));
        }

        [Test]
        public void ShouldPassAsScenarioStep()
        {
            foreach (var step in _actionStep.ScenarioSteps)
            {
                Assert.That(_actionStep.IsScenarioStep(step + " X"), Is.True,
                    string.Format("'{0}' should be a scenario step", step));
            }
        }
    }
}
