using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NBehave.Fluent.Extensions;
using NBehave.Narrator.Framework;

namespace NBehave.Spec.MSTest10.Specifications
{
    [TestClass]
    public class ScenarioDrivenSpecBaseSpecs : MSTest10.ScenarioDrivenSpecBase
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
            Feature.Narrative.ShouldEqual("As a developer, I want to specify a feature so that I can specify behaviour through scenarios");
        }

        [TestMethod]
        public void should_execute_scenarios_implemented_inline()
        {
            string detail1 = null;
            string detail2 = null;
            Feature.AddScenario()
                .Given("a scenario with inline implementation",         () => detail1 = "implementation")
                .When("the scenario is executed",                       () => detail2 = "implementation")
                .Then("the implementation from Given should be called", () => detail1.ShouldNotBeNull())
                .And("the implementation from When should be called",   () => detail2.ShouldNotBeNull());
        }
    }
}
