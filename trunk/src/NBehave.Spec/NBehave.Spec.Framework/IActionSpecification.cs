using System;

namespace NBehave.Spec.Framework
{
    public interface IActionSpecification<T>
    {
        void WhenCalling(Action<T> action);
    }
}