using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace NBehave.Narrator.Framework.Specifications
{
    [TestFixture]
    public class ActionStepSpec
    {
        private ActionStep _actionStep;

        [SetUp]
        public void Establish_context()
        {
            _actionStep = new ActionStep();
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
        public void Colon_should_be_allow_after_scenario()
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
            Assert.That(_actionStep.IsStoryTitle("Story A story"), Is.True);
        }

        [Test]
        public void Should_not_be_a_story_title()
        {
            Assert.That(_actionStep.IsStoryTitle("Foo: A story"), Is.False);
        }

        [Test]
        public void Colon_should_be_allow_after_story()
        {
            Assert.That(_actionStep.IsStoryTitle("Story: A story"), Is.True);
        }

        [Test]
        public void Should_Get_title_of_story()
        {
            Assert.That(_actionStep.GetTitle("Story: A story"), Is.EqualTo("A story"));
        }

        [Test]
        public void Should_pass_as_narrative()
        {
            foreach (var narrative in ActionStep.StoryNarrative)
            {
                Assert.That(_actionStep.IsNarrative(narrative+ " X"), Is.True, 
                    string.Format("'{0}' should be a narrative step", narrative));
            }
        }

        [Test]
        public void Should_pass_as_scenario_step()
        {
            foreach (var step in ActionStep.ScenarioSteps)
            {
                Assert.That(_actionStep.IsScenarioStep(step + " X"), Is.True, 
                    string.Format("'{0}' should be a scenario step", step));
            }
        }
    }
}
