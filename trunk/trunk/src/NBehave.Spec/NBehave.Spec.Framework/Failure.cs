using System;
using System.Text;

namespace NBehave.Spec.Framework
{
    public class Failure
    {
        public object Actual;
        public string ContextName;
        public Exception Exception;
        public object Expected;
        public string SpecificationName;

        public override string ToString()
        {
            string expected = Expected == null ? "null" : Expected.ToString();
            string actual = Actual == null ? "null" : Actual.ToString();

            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.Append(ContextName);
            stringBuilder.Append('.');
            stringBuilder.Append(SpecificationName);
            stringBuilder.Append("\n\r\n\r");

            if (Exception == null)
            {
                stringBuilder.Append("\tExpected\t: " + expected.ToString());
                stringBuilder.Append("\n\r\tActual\t\t: " + actual.ToString());
            }
            else
            {
                stringBuilder.Append("\tException\t: " + Exception.GetType().Name);
                stringBuilder.Append("\n\r\tMessage\t\t: " + Exception.Message);
            }

            stringBuilder.Append("\n\r\n\r");

            return stringBuilder.ToString();
        }
    }
}