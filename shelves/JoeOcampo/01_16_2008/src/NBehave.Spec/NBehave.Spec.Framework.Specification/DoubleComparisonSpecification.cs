using System;

namespace NBehave.Spec.Framework.Specification
{
    [Context]
    public class doubleingPointComparisonSpecification : ComparisonSpecificationBase
    {
        [Specification]
        public void ShouldRegisterSuccessWithSuccessfulCompare()
        {
            double delta = 0.1f;
            double f1 = 1f;
            double f2 = f1 + delta;

            Specify.DontBroadcastNextSpec();
            Specify.That(f2).ShouldEqual(f1).WithAToleranceOf(0.11f);

            Runner.AddLastSpecification();
            Runner.RunComparisons();

            VerifySingleSuccess();
        }

        [Specification]
        public void ShouldRegisterSuccessWith2NaNs()
        {
            Specify.DontBroadcastNextSpec();
            Specify.That(double.NaN).ShouldEqual(double.NaN);

            Runner.AddLastSpecification();
            Runner.RunComparisons();

            VerifySingleSuccess();
        }

        [Specification]
        public void ShouldRegisterSuccessWith2EqualInfinities()
        {
            Specify.DontBroadcastNextSpec();
            Specify.That(double.PositiveInfinity).ShouldEqual(double.PositiveInfinity);
            Runner.AddLastSpecification();

            Specify.DontBroadcastNextSpec();
            Specify.That(double.NegativeInfinity).ShouldEqual(double.NegativeInfinity);
            Runner.AddLastSpecification();

            Runner.RunComparisons();

            VerifySuccessAndFailureCount(2, 0);
        }

        [Specification]
        public void ShouldRegisterFailureWith2Unequaldoubles()
        {
            double f1 = 1f;
            double f2 = 2f;
            double tolerance = .5f;

            Specify.DontBroadcastNextSpec();
            Specify.That(f1).ShouldEqual(f2).WithAToleranceOf(tolerance);

            Runner.AddLastSpecification();
            Runner.RunComparisons();

            VerifySingleFailure();
        }

        [Specification]
        public void ShouldRegisterFailureWith1Nan()
        {
            double f1 = 1f;
            double f2 = double.NaN;

            Specify.DontBroadcastNextSpec();
            Specify.That(f2).ShouldEqual(f1);

            Runner.AddLastSpecification();
            Runner.RunComparisons();

            VerifySingleFailure();
        }
    }
}