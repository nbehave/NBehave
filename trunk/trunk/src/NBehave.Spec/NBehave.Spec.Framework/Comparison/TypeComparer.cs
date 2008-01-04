using System;

namespace NBehave.Spec.Framework
{
    public class TypeComparer : Comparer
    {
        internal TypeComparer(Type expected, object actual)
            : base(expected, actual)
        {
        }

        protected override bool DoComparesOK()
        {
            if (actual == null)
                return false;

            actual = actual.GetType();

            return expected.Equals(actual);
        }
    }
}