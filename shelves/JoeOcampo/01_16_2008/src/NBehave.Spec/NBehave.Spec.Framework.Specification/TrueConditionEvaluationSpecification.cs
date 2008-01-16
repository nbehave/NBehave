using System;

namespace NBehave.Spec.Framework.Specification
{
    [Context]
    public class TrueConditionEvaluationSpecification : ComparisonSpecificationBase
    {
        [Specification]
        public void ShouldRegisterSuccessWhenTrue()
        {
            Specify.DontBroadcastNextSpec();
            Specify.That(true).ShouldBeTrue();
            Runner.AddLastSpecification();
            Runner.RunComparisons();

            VerifySingleSuccess();
        }

        [Specification]
        public void ShouldRegisterFailureWhenFalse()
        {
            Specify.DontBroadcastNextSpec();
            Specify.That(false).ShouldBeTrue();
            Runner.AddLastSpecification();
            Runner.RunComparisons();

            VerifySingleFailure();
        }
    }
}