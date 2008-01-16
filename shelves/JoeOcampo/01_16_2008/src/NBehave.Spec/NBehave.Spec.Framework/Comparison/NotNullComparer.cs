using System;

namespace NBehave.Spec.Framework
{
    public class NotNullComparer : Comparer
    {
        internal NotNullComparer(object actual)
            : base(null, actual)
        {
        }

        protected override bool DoComparesOK()
        {
            return actual != null;
        }
    }
}