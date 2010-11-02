using System;

namespace NBehave.Spec.Framework.Specification
{
    [Context]
    public class Int32ComparisonSpecification : ComparisonSpecificationBase
    {
        [Specification]
        public void ShouldRegisterSuccessWithSuccessfulCompare()
        {
            int i1 = 1;
            int i2 = 1;

            Specify.DontBroadcastNextSpec();
            Specify.That(i2).ShouldEqual(i1);

            Runner.AddLastSpecification();
            Runner.RunComparisons();

            VerifySingleSuccess();
        }

        [Specification]
        public void ShouldRegisterSuccessWithTwoMaxValues()
        {
            Specify.DontBroadcastNextSpec();
            Specify.That(int.MaxValue).ShouldEqual(int.MaxValue);

            Runner.AddLastSpecification();
            Runner.RunComparisons();

            VerifySingleSuccess();
        }

        [Specification]
        public void ShouldRegisterSuccessWithTwoMinValues()
        {
            Specify.DontBroadcastNextSpec();
            Specify.That(int.MinValue).ShouldEqual(int.MinValue);
            Runner.AddLastSpecification();

            Runner.RunComparisons();

            VerifySuccessAndFailureCount(1, 0);
        }

        [Specification]
        public void ShouldRegisterFailureWith2Unequalints()
        {
            int i1 = 1;
            int i2 = 2;

            Specify.DontBroadcastNextSpec();
            Specify.That(i1).ShouldEqual(i2);

            Runner.AddLastSpecification();
            Runner.RunComparisons();

            VerifySingleFailure();
        }
    }
}