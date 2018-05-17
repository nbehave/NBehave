using System;
using System.Runtime.Serialization;
using System.Text;

namespace NBehave
{
    [Serializable]
    public class WrappedException : Exception
    {
        private readonly string message;
        private readonly string stackTrace;
        private readonly string toString;

        public WrappedException(Exception exception)
        {
            var we = exception as WrappedException;
            if (we != null)
            {
                message = we.message;
                stackTrace = we.stackTrace;
                toString = we.toString;
            }
            else
            {
                message = BuildMessage(exception); //exception.Message
                stackTrace = BuildStackTrace(exception); //exception.Exception
                toString = exception.ToString();
            }
        }

        protected WrappedException(SerializationInfo info, StreamingContext context)
        { }

        public override string Message { get { return message; } }
        public override string StackTrace { get { return stackTrace; } }


        private string BuildMessage(Exception exception)
        {
            var sb = new StringBuilder();
            sb.AppendFormat("{0} : {1}", exception.GetType(), exception.Message);

            var inner = exception.InnerException;
            while (inner != null)
            {
                sb.AppendLine();
                sb.AppendFormat("  ----> {0} : {1}", inner.GetType(), inner.Message);
                inner = inner.InnerException;
            }

            return sb.ToString();
        }

        private string BuildStackTrace(Exception exception)
        {
            var builder = new StringBuilder(exception.StackTrace);

            var inner = exception.InnerException;
            while (inner != null)
            {
                builder.AppendLine();
                builder.Append("--");
                builder.Append(inner.GetType().Name);
                builder.AppendLine();
                builder.Append(inner.StackTrace);
                inner = inner.InnerException;
            }

            return builder.ToString();
        }

        public override string ToString()
        {
            return toString;
        }
    }
}