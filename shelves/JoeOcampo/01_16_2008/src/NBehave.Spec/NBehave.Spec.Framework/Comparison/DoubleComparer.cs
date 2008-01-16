using System;

namespace NBehave.Spec.Framework
{
    public class DoubleComparer : GenericComparer<double>
    {
        private double _tolerance;

        internal DoubleComparer(double expected, object actual)
            : base(expected, Convert.ToDouble(actual))
        {
        }

        protected override bool DoComparesOK()
        {
            if (base.DoComparesOK())
                return true;

            return Math.Abs(SpecificExpected - SpecificActual) <= _tolerance;
        }

        public void WithAToleranceOf(double tolerance)
        {
            this._tolerance = tolerance;
        }
    }
}