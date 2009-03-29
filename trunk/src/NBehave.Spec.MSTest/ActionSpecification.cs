using System;

namespace NBehave.Spec.MSTest
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
            Exception e = null;

            try
            {
                action(_value);
            }
            catch (Exception ex)
            {
                e = ex;
            }

            e.ShouldNotBeNull();
            e.ShouldBeInstanceOf(_exceptionType);
        }
    }
}