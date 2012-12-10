using NBehave.Fluent.Framework.Extensions;
using NBehave.Fluent.Framework.NUnit;

using NBehave.Spec.NUnit;
using NUnit.Framework;

namespace NBehave.Examples.Calculator_table
{
    [TestFixture]
    public class CalculatorSpecWithInlineImplementation : ScenarioDrivenSpecBase
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
            Calculator calculator = null;

            Feature.AddScenario()
                .Given("a calculator",                       () => calculator = new Calculator())
                .And("I have entered 1 into the calculator", () => calculator.Enter(1))
                .And("I have entered 1 into the calculator", () => calculator.Enter(1))
                .When("I add the numbers",                   () => calculator.Add())
                .Then("the sum should be  2",                () => calculator.Value().ShouldEqual(2));
        }
    }
}
