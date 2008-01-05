using System;

namespace NBehave.Spec.Framework
{
    public class NullComparer : Comparer
    {
        internal NullComparer(object actual)
            : base(null, actual)
        {
        }

        protected override bool DoComparesOK()
        {
            return actual == null;
        }
    }
}