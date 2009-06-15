using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace NBehave.Narrator.Framework.Specifications
{
    [TestFixture]
    public class ScenarioFixture
    {
        [Test]
        public void Should_set_the_title_when_created()
        {
            Scenario scenario = new Story("Title").WithScenario("Scenario title").Scenario;

            Assert.That(scenario.Title, Is.EqualTo("Scenario title"));
        }

        [Test]
        public void Should_pass_as_a_scenario()
        {
            Assert.That(Scenario.IsScenarioTitle("Scenario A test scenario"), Is.True);
        }

        [Test]
        public void Should_not_be_a_scenario()
        {
            Assert.That(Scenario.IsScenarioTitle("Foo: A test scenario"), Is.False);
        }

        [Test]
        public void Colon_should_be_allow_after_scenario()
        {
            Assert.That(Scenario.IsScenarioTitle("Scenario: A test scenario"), Is.True);
        }

        [Test]
        public void Should_Get_title_of_scenario()
        {
            Assert.That(Scenario.GetTitle("Scenario: A test scenario"), Is.EqualTo("A test scenario"));
        }
    }
}