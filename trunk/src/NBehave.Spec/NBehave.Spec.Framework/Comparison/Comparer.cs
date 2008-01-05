using System;

namespace NBehave.Spec.Framework
{
    public abstract class Comparer
    {
        protected object actual;
        protected object expected;
        internal Failure Failure;

        protected Comparer(object expected, object actual)
        {
            this.expected = expected;
            this.actual = actual;
        }

        protected void Fail()
        {
            Failure = new Failure();
            Failure.Expected = expected;
            Failure.Actual = actual;
        }

        protected abstract bool DoComparesOK();

        internal bool ComparesOK()
        {
            bool result = DoComparesOK();

            if (!result)
            {
                Fail();
            }

            return result;
        }
    }
}