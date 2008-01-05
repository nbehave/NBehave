using System;

namespace NBehave.Spec.Framework
{
    public class BooleanComparer : Comparer
    {
        internal BooleanComparer(bool expected, object actual)
            : base(expected, actual)
        {
        }

        protected override bool DoComparesOK()
        {
            return actual is bool && (bool) actual == (bool) expected;
        }
    }
}