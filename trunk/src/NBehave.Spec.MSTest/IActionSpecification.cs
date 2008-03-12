using System;

namespace NBehave.Spec.MSTest
{
    public interface IActionSpecification<T>
    {
        void WhenCalling(Action<T> action);
    }
}