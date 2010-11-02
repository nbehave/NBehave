using System;

namespace NBehave.Spec.Framework.Specification
{
    [Context]
    public class ReferentialEqualityEvaluationSpecification : ComparisonSpecificationBase
    {
        [Specification]
        public void ShouldRegisterSuccessWhenReferentiallyEqual()
        {
            object o = new object();
            Specify.DontBroadcastNextSpec();
            Specify.That(o).ShouldBeTheSameAs(o);
            Runner.AddLastSpecification();
            Runner.RunComparisons();

            VerifySingleSuccess();
        }

        [Specification]
        public void ShouldRegisterFailureWhenReferentiallyUnequal()
        {
            Specify.DontBroadcastNextSpec();
            Specify.That(new object()).ShouldBeTheSameAs(new object());
            Runner.AddLastSpecification();
            Runner.RunComparisons();

            VerifySingleFailure();
        }
    }
}