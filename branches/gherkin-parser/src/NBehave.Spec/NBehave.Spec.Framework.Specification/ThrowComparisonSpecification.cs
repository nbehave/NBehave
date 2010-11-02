using System;

namespace NBehave.Spec.Framework.Specification
{
    [Context]
    public class ThrowComparisonSpecification : ComparisonSpecificationBase
    {
        [Specification]
        public void ShouldRegisterSuccessWhenCorrectExceptionIsThrown()
        {
            MethodThatThrows mtt = delegate() { new Thrower().Throw(); };

            Specify.DontBroadcastNextSpec();
            Specify.ThrownBy(mtt).ShouldBeOfType(typeof (IndexOutOfRangeException));

            Runner.AddLastSpecification();
            Runner.RunComparisons();

            VerifySingleSuccess();
        }

        [Specification]
        public void ShouldRegisterSingleFailureWhenIncorrectExceptionIsThrown()
        {
            MethodThatThrows mtt = delegate() { new Thrower().Throw(); };

            Specify.DontBroadcastNextSpec();
            Specify.ThrownBy(mtt).ShouldBeOfType(typeof (ArgumentException));

            Runner.AddLastSpecification();
            Runner.RunComparisons();

            VerifySingleFailure();
        }

        [Specification]
        public void ShouldRegisterSingleFailureWhenNoExceptionIsThrown()
        {
            MethodThatThrows mtt = delegate()
                                       {
                                           // Do nothing.
                                       };

            Specify.DontBroadcastNextSpec();
            Specify.ThrownBy(mtt).ShouldBeOfType(typeof (ArgumentException));

            Runner.AddLastSpecification();
            Runner.RunComparisons();

            VerifySingleFailure();
        }
    }

    internal class Thrower
    {
        internal void Throw()
        {
            throw new IndexOutOfRangeException();
        }
    }
}