using System;

namespace NBehave.Spec.Framework.Specification
{
    [Context]
    public class FalseConditionEvaluationSpecification : ComparisonSpecificationBase
    {
        [Specification]
        public void ShouldRegisterSuccessWithFalseResult()
        {
            Specify.DontBroadcastNextSpec();
            Specify.That(false).ShouldBeFalse();

            Runner.AddLastSpecification();
            Runner.RunComparisons();

            VerifySingleSuccess();
        }

        [Specification]
        public void ShouldRegisterFailureWithTrueResult()
        {
            Specify.DontBroadcastNextSpec();
            Specify.That(true).ShouldBeFalse();

            Runner.AddLastSpecification();
            Runner.RunComparisons();

            VerifySingleFailure();
        }
    }
}