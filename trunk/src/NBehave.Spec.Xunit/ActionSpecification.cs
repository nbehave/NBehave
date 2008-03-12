using System;
using NBehave.Spec.Framework;
using Xunit;

namespace NBehave.Spec.Xunit
{
    public class ActionSpecification<T> : IActionSpecification<T>
    {
        private readonly T _value;
        private readonly Type _exceptionType;

        public ActionSpecification(T value, Type exceptionType)
        {
            _value = value;
            _exceptionType = exceptionType;
        }

        public void WhenCalling(Action<T> action)
        {
            Assert.ThrowsDelegate d = () => action(_value);
            Assert.Throws(_exceptionType, d);
        }
    }
}