using System;
using System.Collections.Generic;
using System.Text;

namespace NBehave.Spec.Framework
{
    public class GenericComparer<T> : Comparer
        where T : IComparable<T>
    {
        private readonly T specificExpected;
        private readonly T specificActual;

        public GenericComparer(T expected, T actual)
            : base(expected, actual)
        {
            specificExpected = expected;
            specificActual = actual;
        }

        protected T SpecificActual
        {
            get { return specificActual; }
        }

        protected T SpecificExpected
        {
            get { return specificExpected; }
        }

        protected override bool DoComparesOK()
        {
            return specificExpected.CompareTo(specificActual) == 0;
        }
    }
}
