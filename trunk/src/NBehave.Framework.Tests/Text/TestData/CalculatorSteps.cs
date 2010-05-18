using NUnit.Framework;
using System;

namespace NBehave.Narrator.Framework.Specifications.Text.TestData
{
    [ActionSteps]
    public class CalculatorSteps
    {
        private static int _left = -1;
        private static int _right = -1;
        private static int _sum = -1;


        public int Left
        {
            get { return _left; }
            set { _left = value; }
        }

        public int Right
        {
            get { return _right; }
            set { _right = value; }
        }

        public int Sum
        {
            get { return _sum; }
            set { _sum = value; }
        }

        [Given("numbers $left and $right")]
        public void Numbers(int left, int right)
        {
            Left = left;
            Right = right;
        }

        [When("I add the numbers")]
        public void Add()
        {
            Sum = Left + Right;
        }

        [Then("the sum is $sum")]
        public void SumIs(int sum)
        {
            Assert.That(sum, Is.EqualTo(Sum));
        }
    }
}
