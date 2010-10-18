using NBehave.Narrator.Framework;
using NBehave.Spec.NUnit;

namespace NBehave.Examples.Calculator_table.Steps
{
    [ActionSteps]
    public class AddNumbers
    {
        private Calculator _calculator;

        [Given("a calculator")]
        public void SetUp_scenario()
        {
            _calculator = new Calculator();
        }

        [Given(@"I have entered $number into the calculator")]
        public void Enter_number(int number)
        {
            _calculator.Enter(number);
        }

        [When(@"I add the numbers")]
        public void Add()
        {
            _calculator.Add();
        }

        [Then(@"the sum should be $result")]
        public void Result(int result)
        {
            _calculator.Value().ShouldEqual(result);
        }
    }
}
