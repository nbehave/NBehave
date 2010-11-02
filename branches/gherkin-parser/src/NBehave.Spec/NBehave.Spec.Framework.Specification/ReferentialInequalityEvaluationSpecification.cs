using System;

namespace NBehave.Spec.Framework.Specification
{
    [Context]
    public class ReferentialInequalityEvaluationSpecification : ComparisonSpecificationBase
    {
        [Specification]
        public void ShouldRegisterSuccessWhenReferentiallyUnequal()
        {
            Specify.DontBroadcastNextSpec();
            Specify.That(new object()).ShouldNotBeTheSameAs(new object());
            Runner.AddLastSpecification();
            Runner.RunComparisons();

            VerifySingleSuccess();
        }

        [Specification]
        public void ShouldRegisterFailureWhenReferentiallyEqual()
        {
            object o = new object();
            Specify.DontBroadcastNextSpec();
            Specify.That(o).ShouldNotBeTheSameAs(o);
            Runner.AddLastSpecification();
            Runner.RunComparisons();

            VerifySingleFailure();
        }
    }
}