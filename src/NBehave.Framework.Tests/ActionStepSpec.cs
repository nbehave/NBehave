using System.Collections.Generic;
using NUnit.Framework;

namespace NBehave.Narrator.Framework.Specifications
{
    [TestFixture]
    public class ActionStepSpec
    {
        private ActionStepVerifier _actionStepVerifier;

        [SetUp]
        public void EstablishContext()
        {
            var lang = new List<Language>();
            var en = BuildEnglish();
            lang.Add(en);
            _actionStepVerifier = new ActionStepVerifier(lang);
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
            Assert.That(_actionStepVerifier.IsScenarioTitle("Scenario A test scenario"), Is.True);
        }

        [Test]
        public void ShouldNotBeAScenarioTitle()
        {
            Assert.That(_actionStepVerifier.IsScenarioTitle("Foo: A test scenario"), Is.False);
        }

        [Test]
        public void ColonShouldBeAllowedAfterScenario()
        {
            Assert.That(_actionStepVerifier.IsScenarioTitle("Scenario: A test scenario"), Is.True);
        }

        [Test]
        public void ShouldGetTitleOfScenario()
        {
            Assert.That(_actionStepVerifier.GetTitle("Scenario: A test scenario"), Is.EqualTo("A test scenario"));
        }

        [Test]
        public void ShouldPassAsAStoryTitle()
        {
            Assert.That(_actionStepVerifier.IsFeatureTitle("Story A story"), Is.True);
        }

        [Test]
        public void ShouldNotBeAStoryTitle()
        {
            Assert.That(_actionStepVerifier.IsFeatureTitle("Foo: A story"), Is.False);
        }

        [Test]
        public void ColonShouldBeAllowedAfterStory()
        {
            Assert.That(_actionStepVerifier.IsFeatureTitle("Story: A story"), Is.True);
        }

        [Test]
        public void ShouldGetTitleOfStory()
        {
            Assert.That(_actionStepVerifier.GetTitle("Story: A story"), Is.EqualTo("A story"));
        }

        [Test]
        public void ShouldPassAsScenarioStep()
        {
            foreach (var step in _actionStepVerifier.ScenarioSteps)
            {
                Assert.That(_actionStepVerifier.IsScenarioStep(step + " X"), Is.True,
                    string.Format("'{0}' should be a scenario step", step));
            }
        }
    }
}
