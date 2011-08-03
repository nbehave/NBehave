using System;

namespace NBehave.Fluent
{
    public interface IActionSpecification<T>
    {
        void WhenCalling(Action<T> action);
    }
}