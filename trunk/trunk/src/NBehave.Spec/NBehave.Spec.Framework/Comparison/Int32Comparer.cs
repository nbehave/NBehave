using System;

namespace NBehave.Spec.Framework
{
    public class Int32Comparer : GenericComparer<int>
    {
        internal Int32Comparer(int expected, object actual)
            : base(expected, Convert.ToInt32(actual))
        {
        }
    }
}