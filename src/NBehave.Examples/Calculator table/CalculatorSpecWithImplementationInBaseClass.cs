using NBehave;
using NBehave.Fluent.Framework.Extensions;
using NBehave.Fluent.Framework.NUnit;
using NBehave.Spec.NUnit;
using NUnit.Framework;

namespace NBehave.Examples.Calculator_table
{
    [TestFixture]
    public class CalculatorSpecWithImplementationInBaseClass : CalculatorAdditionSpec
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
            var stepHelper = this;
            Feature.AddScenario()
                .WithHelperObject(stepHelper)
                .Given("a calculator")
                .And("I have entered 1 into the calculator")
                .And("I have entered 1 into the calculator")
                .When("I add the numbers")
                .Then("the sum should be 2");
        }
    }

    public abstract class CalculatorAdditionSpec : ScenarioDrivenSpecBase
    {
        private Calculator _calculator;
        
        protected void Given_a_calculator()
        {
            _calculator = new Calculator();
        }

        protected void Given_I_have_entered_1_into_the_calculator()
        {
            _calculator.Enter(1);
        }

        protected void When_I_add_the_numbers()
        {
            _calculator.Add();
        }

        protected void Then_the_sum_should_be_2()
        {
            _calculator.Value().ShouldEqual(2);
        }
    }
}
