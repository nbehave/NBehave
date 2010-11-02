using System;

namespace NBehave.Spec.Framework.Specification
{
    [Context]
    public class NotNullEvaluationSpecification : ComparisonSpecificationBase
    {
        [Specification]
        public void ShouldRegisterSuccessWhenNotNull()
        {
            Specify.DontBroadcastNextSpec();
            Specify.That(new object()).ShouldNotBeNull();

            Runner.AddLastSpecification();
            Runner.RunComparisons();

            VerifySingleSuccess();
        }

        [Specification]
        public void ShouldRegisterFailureWhenNull()
        {
            Specify.DontBroadcastNextSpec();
            Specify.That(null).ShouldNotBeNull();

            Runner.AddLastSpecification();
            Runner.RunComparisons();

            VerifySingleFailure();
        }
    }
}