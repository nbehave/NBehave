using NBehave.Narrator.Framework;
using NBehave.Spec.Extensions;
using NBehave.Spec.NUnit;
using NUnit.Framework;

namespace NUnit.SpecBase_Specifications
{
    [TestFixture]
    public class ScenarioDrivenSpecBaseSpecs : ScenarioDrivenSpecBase
    {
        protected override Feature CreateFeature()
        {
            return new Feature("Setting up features and scenarios")
                .AddStory()
                .AsA("developer")
                .IWant("to specify a feature")
                .SoThat("I can specify behaviour through scenarios");
        }

        [Test]
        public void should_populate_the_feature_narrative()
        {
            Feature.Narrative.ShouldEqual("As a developer, I want to specify a feature so that I can specify behaviour through scenarios");
        }

        [Test]
        public void should_execute_scenarios_implemented_inline()
        {
            string detail1 = null;
            string detail2 = null;
            Feature.AddScenario()
                .Given("a scenario with inline implementation", () => detail1 = "implementation")
                .When("the scenario is executed", () => detail2 = "implementation")
                .Then("the implementation from Given should be called", () => detail1.ShouldNotBeNull())
                .And("the implementation from When should be called", () => detail2.ShouldNotBeNull());
            Assert.IsNotNull(detail1);
            Assert.IsNotNull(detail2);
        }
    }
}
