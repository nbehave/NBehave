using System;
using System.Collections;
using MbUnit.Framework;

namespace NBehave.Spec.MbUnit
{
    public static class Extensions
    {
        public static void ShouldBeTrue(this bool condition)
        {
            Assert.IsTrue(condition);
        }

        public static void ShouldBeFalse(this bool condition)
        {
            Assert.IsFalse(condition);
        }

        public static void ShouldEqual(this object actual, object expected)
        {
            Assert.AreEqual(actual, expected);
        }

        public static void ShouldNotEqual(this object actual, object expected)
        {
            Assert.AreNotEqual(actual, expected);
        }

        public static void ShouldBeTheSameAs(this object actual, object expected)
        {
            Assert.AreSame(actual, expected);
        }

        public static void ShouldNotBeTheSameAs(this object actual, object expected)
        {
            Assert.AreNotSame(actual, expected);
        }

        public static void ShouldContain(this ICollection actual, object expected)
        {
            CollectionAssert.Contains(actual, expected);
        }

        public static void ShouldBeGreaterThan(this IComparable arg1, IComparable arg2)
        {
            Assert.GreaterThan(arg1, arg2);
        }

        public static void ShouldBeGreaterThanOrEqualTo(this IComparable arg1, IComparable arg2)
        {
            Assert.GreaterEqualThan(arg1, arg2);
        }

        public static void ShouldBeLessThan(this IComparable arg1, IComparable arg2)
        {
            Assert.LowerThan(arg1, arg2);
        }

        public static void ShouldBeLessThanOrEqualTo(this IComparable arg1, IComparable arg2)
        {
            Assert.LowerEqualThan(arg1, arg2);
        }

        public static void ShouldBeAssignableFrom(this object actual, Type expected)
        {
            Assert.IsAssignableFrom(expected, actual);
        }

        public static void ShouldBeAssignableFrom<ExpectedType>(this Object actual)
        {
            actual.ShouldBeAssignableFrom(typeof(ExpectedType));
        }

        public static void ShouldNotBeAssignableFrom(this object actual, Type expected)
        {
            Assert.IsNotAssignableFrom(expected, actual);
        }

        public static void ShouldNotBeAssignableFrom<ExpectedType>(this object actual)
        {
            actual.ShouldNotBeAssignableFrom(typeof(ExpectedType));
        }

        public static void ShouldBeEmpty(this string value)
        {
            Assert.IsEmpty(value);
        }

        public static void ShouldNotBeEmpty(this string value)
        {
            Assert.IsNotEmpty(value);
        }

        public static void ShouldBeEmpty(this ICollection collection)
        {
            CollectionAssert.AreCountEqual(0, collection);
        }

        public static void ShouldNotBeEmpty(this ICollection collection)
        {
            Assert.IsTrue(collection.Count > 0);
        }

        public static void ShouldBeInstanceOfType(this object actual, Type expected)
        {
            Assert.IsInstanceOfType(expected, actual);
        }

        public static void ShouldBeInstanceOf<ExpectedType>(this object actual)
        {
            actual.ShouldBeInstanceOfType(typeof(ExpectedType));
        }

        public static void ShouldNotBeInstanceOfType(this object actual, Type expected)
        {
            Assert.IsNotInstanceOfType(expected, actual);
        }

        public static void ShouldNotBeInstanceOf<ExpectedType>(this object actual)
        {
            actual.ShouldNotBeInstanceOfType(typeof(ExpectedType));
        }

        public static void ShouldBeNaN(this double value)
        {
            Assert.IsNaN(value);
        }

        public static void ShouldBeNull(this object value)
        {
            Assert.IsNull(value);
        }

        public static void ShouldNotBeNull(this object value)
        {
            Assert.IsNotNull(value);
        }

        public static void ShouldBeThrownBy(this Type exceptionType, Action action)
        {
            Exception e = null;

            try
            {
                action();
            }
            catch (Exception ex)
            {
                e = ex;
            }

            e.ShouldNotBeNull();
            e.ShouldBeInstanceOfType(exceptionType);
        }

    }
}
