using Microsoft.VisualStudio.TestTools.UnitTesting;
using NBehave.Fluent.Framework.Extensions;

namespace NBehave.Fluent.Framework.Specifications
{
    public abstract class MsTest_ScenarioDrivenSpecBaseSpecs : MSTest.ScenarioDrivenSpecBase
    {
        [TestClass]
        public class ScenarioDrivenSpecBaseSpecs : MsTest_ScenarioDrivenSpecBaseSpecs
        {
            protected override Feature CreateFeature()
            {
                return new Feature("Setting up features and scenarios")
                    .AddStory()
                    .AsA("developer")
                    .IWant("to specify a feature")
                    .SoThat("I can specify behaviour through scenarios");
            }

            [TestMethod]
            public void should_populate_the_feature_narrative()
            {
                Assert.AreEqual("As a developer, I want to specify a feature so that I can specify behaviour through scenarios", Feature.Narrative);
            }

            [TestMethod]
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