using System;
using MbUnit.Framework;
using NBehave.Narrator.Framework;
using NBehave.Spec.Extensions;

namespace NBehave.Spec.MbUnit.Specs
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
        }

        [Test]
        public void should_call_notification_events_before_executing_inline_implementation()
        {
            string lastLoggedScenario = null;
            EventHandler<EventArgs<ActionStepText>> logger = (sender, e) => lastLoggedScenario = e.EventData.Step;
            ScenarioWithSteps.StepAdded += logger;

            try
            {
                Feature.AddScenario()
                    .Given("a scenario with inline implementation", () => lastLoggedScenario.ShouldEqual("a scenario with inline implementation"))
                    .When("the scenario is executed", () => lastLoggedScenario.ShouldEqual("the scenario is executed"))
                    .Then("the implementation from Given should be called", () => lastLoggedScenario.ShouldEqual("the implementation from Given should be called"))
                    .And("the implementation from When should be called", () => lastLoggedScenario.ShouldEqual("the implementation from When should be called"));
            }
            finally
            {
                ScenarioWithSteps.StepAdded -= logger;
            }
        }
    }
}
