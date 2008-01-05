using System;

namespace NBehave.Spec.Framework
{
    public class ReferentialEqualityComparer : Comparer
    {
        internal ReferentialEqualityComparer(object expected, object actual)
            : base(expected, actual)
        {
        }

        protected override bool DoComparesOK()
        {
            return ReferenceEquals(expected, actual);
        }
    }
}