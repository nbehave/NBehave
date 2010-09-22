using System;
using System.Text;

namespace NBehave.Spec.Framework.Specification
{
    [Context]
    public class ObjectComparisonSpecification : ComparisonSpecificationBase
    {
        [Specification]
        public void ShouldRegisterSuccessWith2Nulls()
        {
            Specify.DontBroadcastNextSpec();
            Specify.That(null).ShouldEqual(null);

            Runner.AddLastSpecification();
            Runner.RunComparisons();

            VerifySingleSuccess();
        }

        [Specification]
        public void ShouldRegisterFailureWith1Null()
        {
            Specify.DontBroadcastNextSpec();
            Specify.That(new object()).ShouldEqual(null);
            Runner.AddLastSpecification();
            Runner.RunComparisons();

            VerifySingleFailure();
        }

        [Specification]
        public void ShouldRegisterSuccessWith2EqualInts()
        {
            Specify.DontBroadcastNextSpec();
            Specify.That(5).ShouldEqual(5);
            Runner.AddLastSpecification();
            Runner.RunComparisons();

            VerifySingleSuccess();
        }

        [Specification]
        public void ShouldRegisterFailureWith2UnequalInts()
        {
            Specify.DontBroadcastNextSpec();
            Specify.That(5).ShouldEqual(7);
            Runner.AddLastSpecification();
            Runner.RunComparisons();

            VerifySingleFailure();
        }

        [Specification]
        public void ShouldRegisterSuccessWith2EqualArrays()
        {
            int[] array1 = new int[] {1, 2, 3, 4, 5, 6, 7, 8, 9};
            int[] array2 = new int[] {1, 2, 3, 4, 5, 6, 7, 8, 9};

            Specify.DontBroadcastNextSpec();
            Specify.That(array1).ShouldEqual(array2);
            Runner.AddLastSpecification();
            Runner.RunComparisons();

            VerifySingleSuccess();
        }

        [Specification]
        public void ShouldRegisterFailureWith1ArrayAnd1Object()
        {
            Specify.DontBroadcastNextSpec();
            Specify.That(new object()).ShouldEqual(new object[] {new object(), new object()});
            Runner.AddLastSpecification();
            Runner.RunComparisons();

            VerifySingleFailure();
        }

        [Specification]
        public void ShouldRegisterFailureWith2ArraysOfDifferentLength()
        {
            int[] array1 = new int[] {1, 2, 3, 4, 5, 6, 7, 8};
            int[] array2 = new int[] {1, 2, 3, 4, 5, 6, 7, 8, 9};

            Specify.DontBroadcastNextSpec();
            Specify.That(array1).ShouldEqual(array2);
            Runner.AddLastSpecification();
            Runner.RunComparisons();

            VerifySingleFailure();
        }

        [Specification]
        public void ShouldRegisterSuccessWith2EqualArraysContainingNulls()
        {
            object[] array1 = new object[] {1, 2, 3, 4, 5, null, 7, 8, 9};
            object[] array2 = new object[] {1, 2, 3, 4, 5, null, 7, 8, 9};

            Specify.DontBroadcastNextSpec();
            Specify.That(array1).ShouldEqual(array2);
            Runner.AddLastSpecification();
            Runner.RunComparisons();

            VerifySingleSuccess();
        }

        [Specification]
        public void ShouldRegisterFailureWith2UnequalArrays()
        {
            object[] array1 = new object[] {1, 2, 2, 4, 5, null, 7, 8, 9};
            object[] array2 = new object[] {1, 2, 3, 4, 5, null, 7, 8, 9};

            Specify.DontBroadcastNextSpec();
            Specify.That(array1).ShouldEqual(array2);
            Runner.AddLastSpecification();
            Runner.RunComparisons();

            VerifySingleFailure();
        }

        [Specification]
        public void ShouldRegisterSuccessWith2IdenticalStrings()
        {
            StringBuilder sb1 = new StringBuilder();
            StringBuilder sb2 = new StringBuilder();

            sb1.Append("Test string");
            sb2.Append("Test string");

            Specify.DontBroadcastNextSpec();
            Specify.That(sb1.ToString()).ShouldEqual(sb2.ToString());
            Runner.AddLastSpecification();
            Runner.RunComparisons();

            VerifySingleSuccess();
        }
    }
}