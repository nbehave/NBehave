using System;

namespace NBehave.Spec.MSTest
{
    public interface IActionSpecification
    {
        void ShouldThrow<TException>() where TException : Exception;
    }
}