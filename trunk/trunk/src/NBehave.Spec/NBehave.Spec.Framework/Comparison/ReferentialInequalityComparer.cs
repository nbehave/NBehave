using System;

namespace NBehave.Spec.Framework
{
    public class ReferentialInequalityComparer : Comparer
    {
        internal ReferentialInequalityComparer(object expected, object actual)
            : base(expected, actual)
        {
        }

        protected override bool DoComparesOK()
        {
            return !ReferenceEquals(expected, actual);
        }
    }
}