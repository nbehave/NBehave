using System;

namespace NBehave.Spec.Framework
{
    public class NoMatchingConstructorException : MissingMethodException
    {
        public NoMatchingConstructorException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}