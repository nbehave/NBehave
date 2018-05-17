using NBehave.Fluent.Framework.Extensions;
using NUnit.Framework;

namespace NBehave.Fluent.Framework.Specifications
{
    public abstract class NUnit_ScenarioDrivenSpecBaseSpecs : NUnit.ScenarioDrivenSpecBase
    {
        [TestFixture]
        public class ScenarioDrivenSpecBaseSpecs : NUnit_ScenarioDrivenSpecBaseSpecs
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
                Assert.AreEqual("As a developer, I want to specify a feature so that I can specify behaviour through scenarios", Feature.Narrative);
            }

            [Test]
            public void should_execute_scenarios_implemented_inline()
            {
                string detail1 = null;
                string detail2 = null;
                Feature.AddScenario()
                    .Given("a scenario with inline implementation", () => detail1 = "implementation")
                    .When("the scenario is executed", () => detail2 = "implementation")
                    .Then("the implementation from Given should be called", () => Assert.IsNotNull(detail1))
                    .And("the implementation from When should be called", () => Assert.IsNotNull(detail2));
                Assert.IsNotNull(detail1);
                Assert.IsNotNull(detail2);
            }
        }
    }
}