using System;
using System.Runtime.Serialization;

namespace NBehave.Internal.Gherkin
{
    [Serializable]
    public class ParseException : Exception
    {
        public ParseException(string message, Exception innerException)
            : base(message, innerException)
        {}

        protected ParseException(SerializationInfo info, StreamingContext ctx)
            : base(info, ctx)
        { }

    }
}