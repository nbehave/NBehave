using System;

namespace NBehave.Spec.Framework.Specification
{
    [Context]
    public class NullEvaluationSpecification : ComparisonSpecificationBase
    {
        [Specification]
        public void ShouldRegisterSuccessWhenNull()
        {
            Specify.DontBroadcastNextSpec();
            Specify.That(null).ShouldBeNull();

            Runner.AddLastSpecification();
            Runner.RunComparisons();

            VerifySingleSuccess();
        }

        [Specification]
        public void ShouldRegisterFailureWhenNotNull()
        {
            Specify.DontBroadcastNextSpec();
            Specify.That(new object()).ShouldBeNull();

            Runner.AddLastSpecification();
            Runner.RunComparisons();

            VerifySingleFailure();
        }
    }
}