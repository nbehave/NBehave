using NBehave.Examples.Calculator_table.Steps;
using NBehave.Narrator.Framework;
using NBehave.Spec.Extensions;
using NBehave.Spec.NUnit;
using NUnit.Framework;

namespace NBehave.Examples.Calculator_table
{
    [TestFixture]
    public class CalculatorSpecWithImplementationInHelperClass : ScenarioDrivenSpecBase
    {
        protected override Feature CreateFeature()
        {
            return new Feature("addition of two numbers")
                .AddStory()
                .AsA("user")
                .IWant("my calculator to add number together")
                .SoThat("I don't need to try and do it in my head");
        }

        [Test]
        public void should_add_1_plus_1_correctly()
        {
            Feature.AddScenario()
                .WithHelperObject<AddNumbers>()
                .Given("I have entered 1 into the calculator")
                .And("I have entered 1 into the calculator")
                .When("I add the numbers")
                .Then("the sum should be 2");
        }
    }
}
