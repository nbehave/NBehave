using System;
using System.Collections;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NBehave.Spec.MSTest
{
    public static class Extensions
    {
        public static Exception GetException(this Action action)
        {
            Exception e = null;
            try
            {
                action();
            }
            catch (Exception exc)
            {
                e = exc;
            }
            return e;
        }

        public static void ShouldApproximatelyEqual<T>(this T actual, T expected, T delta)
        {
            if (typeof(T) != typeof(decimal) && typeof(T) != typeof(double) && typeof(T) != typeof(float))
                Assert.Fail("type (T) must be float, double or decimal");

            if (typeof(T) == typeof(decimal))
                Assert.AreEqual(Decimal.ToDouble(Convert.ToDecimal(expected)),
                                Decimal.ToDouble(Convert.ToDecimal(actual)),
                                Decimal.ToDouble(Convert.ToDecimal(delta)));
            if (typeof(T) == typeof(double))
                Assert.AreEqual(Convert.ToDouble(expected), Convert.ToDouble(actual), Convert.ToDouble(delta));
            if (typeof(T) == typeof(float))
                Assert.AreEqual(Convert.ToSingle(expected), Convert.ToSingle(actual), Convert.ToSingle(delta));
        }

        public static void ShouldBeAssignableFrom<TExpectedType>(this Object actual)
        {
            Assert.IsTrue(actual.GetType().IsAssignableFrom(typeof(TExpectedType)),
                string.Format("{0} is not assignable from {1}", actual.GetType().Name, typeof(TExpectedType).Name));
        }

        public static void ShouldBeAssignableFrom(this object actual, Type expected)
        {
            Assert.IsTrue(actual.GetType().IsAssignableFrom(expected),
                string.Format("{0} is not assignable from {1}", actual.GetType().Name, expected.GetType().Name));
        }

        public static void ShouldBeEmpty(this string value)
        {
            Assert.IsTrue(string.IsNullOrEmpty(value), "string should be empty");
        }

        public static void ShouldBeEmpty(this ICollection collection)
        {
            Assert.AreEqual(0, collection.Count, "collection should be empty");
        }

        public static void ShouldBeEqualTo(this ICollection actual, ICollection expected)
        {
            CollectionAssert.AreEqual(expected, actual);
        }

        public static void ShouldBeEquivalentTo(this ICollection actual, ICollection expected)
        {
            CollectionAssert.AreEquivalent(expected, actual);
        }

        public static void ShouldBeFalse(this bool condition)
        {
            Assert.IsFalse(condition);
        }

        public static void ShouldBeGreaterThan(this IComparable arg1, IComparable arg2)
        {
            Assert.IsTrue(arg1.CompareTo(arg2) == 1, string.Format("{0} should be larger than {1}", arg1, arg2));
        }

        public static void ShouldBeGreaterThanOrEqualTo(this IComparable arg1, IComparable arg2)
        {
            Assert.IsTrue(arg1.CompareTo(arg2) >= 0, string.Format("{0} should be larger than or equal to {1}", arg1, arg2));
        }

        [Obsolete("Use ShouldNotBeInstanceOfType")]
        public static void ShouldBeInstanceOf<T>(this object value)
        {
            value.ShouldBeInstanceOfType(typeof(T));
        }

        [Obsolete("Use ShouldNotBeInstanceOfType")]
        public static void ShouldBeInstanceOf(this object value, Type expectedType)
        {
            Assert.IsInstanceOfType(value, expectedType);
        }

        public static void ShouldBeInstanceOfType<T>(this object value)
        {
            value.ShouldBeInstanceOfType(typeof(T));
        }

        public static void ShouldBeInstanceOfType(this object value, Type expectedType)
        {
            Assert.IsInstanceOfType(value, expectedType);
        }

        public static void ShouldBeLessThan(this IComparable arg1, IComparable arg2)
        {
            Assert.IsTrue(arg1.CompareTo(arg2) == -1, string.Format("{0} should be less than {1}", arg1, arg2));
        }

        public static void ShouldBeLessThanOrEqualTo(this IComparable arg1, IComparable arg2)
        {
            Assert.IsTrue(arg1.CompareTo(arg2) <= 0, string.Format("{0} should be less than or equal to {1}", arg1, arg2));
        }

        public static void ShouldBeNaN(this double value)
        {
            Assert.IsTrue(double.IsNaN(value), "value should be NaN");
        }

        public static void ShouldBeNull(this object value)
        {
            Assert.IsNull(value);
        }

        public static void ShouldBeTheSameAs(this object actual, object expected)
        {
            Assert.AreSame(expected, actual);
        }

        public static void ShouldBeThrownBy(this Type exceptionType, ThrowingAction expressionThatThrows)
        {
            Exception e = null;

            try
            {
                expressionThatThrows.Invoke();
            }
            catch (Exception ex)
            {
                e = ex;
            }

            e.ShouldNotBeNull();
            e.ShouldBeInstanceOfType(exceptionType);
        }

        public static void ShouldBeTrue(this bool condition)
        {
            Assert.IsTrue(condition);
        }

        public static void ShouldContain(this string value, string substring)
        {
            StringAssert.Contains(value, substring);
        }

        public static void ShouldContain(this ICollection collection, object element)
        {
            CollectionAssert.Contains(collection, element);
        }

        public static void ShouldEndWith(this string value, string substring)
        {
            StringAssert.EndsWith(value, substring);
        }

        public static void ShouldEqual<T>(this T actual, T expected)
        {
            Assert.AreEqual(expected, actual);
        }

        public static void ShouldMatch(this string value, Regex pattern)
        {
            StringAssert.Matches(value, pattern);
        }

        public static void ShouldMatch(this string actual, string regexPattern, RegexOptions regexOptions)
        {
            Regex r = new Regex(regexPattern, regexOptions);
            ShouldMatch(actual, r);
        }

        public static void ShouldNotBeAssignableFrom<TExpectedType>(this Object actual)
        {
            Assert.IsFalse(actual.GetType().IsAssignableFrom(typeof(TExpectedType)),
                string.Format("{0} is assignable from {1}", actual.GetType().Name, typeof(TExpectedType).Name));
        }

        public static void ShouldNotBeAssignableFrom(this object actual, Type expected)
        {
            Assert.IsFalse(actual.GetType().IsAssignableFrom(expected),
                string.Format("{0} is assignable from {1}", actual.GetType().Name, expected.GetType().Name));
        }

        public static void ShouldNotBeEmpty(this string value)
        {
            Assert.IsFalse(string.IsNullOrEmpty(value), "string should not be empty");
        }

        public static void ShouldNotBeEmpty(this ICollection collection)
        {
            Assert.AreNotEqual(0, collection.Count, "collection should not be empty");
        }

        public static void ShouldNotBeEqualTo(this ICollection actual, ICollection expected)
        {
            CollectionAssert.AreNotEqual(expected, actual);
        }

        public static void ShouldNotBeEquivalentTo(this ICollection actual, ICollection expected)
        {
            CollectionAssert.AreNotEquivalent(expected, actual);
        }

        [Obsolete("Use ShouldNotBeInstanceOfType")]
        public static void ShouldNotBeInstanceOf<T>(this object value)
        {
            value.ShouldNotBeInstanceOfType(typeof(T));
        }

        [Obsolete("Use ShouldNotBeInstanceOfType")]
        public static void ShouldNotBeInstanceOf(this object value, Type wrongType)
        {
            Assert.IsNotInstanceOfType(value, wrongType);
        }

        public static void ShouldNotBeInstanceOfType<T>(this object value)
        {
            value.ShouldNotBeInstanceOfType(typeof(T));
        }

        public static void ShouldNotBeInstanceOfType(this object value, Type wrongType)
        {
            Assert.IsNotInstanceOfType(value, wrongType);
        }

        public static void ShouldNotBeNull(this object value)
        {
            Assert.IsNotNull(value);
        }

        public static void ShouldNotBeTheSameAs(this object actual, object notExpected)
        {
            Assert.AreNotSame(notExpected, actual);
        }

        public static void ShouldNotContain(this ICollection collection, object element)
        {
            CollectionAssert.DoesNotContain(collection, element);
        }

        public static void ShouldNotContain(this string actual, string expected)
        {
            Assert.IsTrue(actual.IndexOf(expected) == -1, string.Format("{0} should not contain {1}", actual, expected));
        }

        public static void ShouldNotEqual<T>(this T actual, T notExpected)
        {
            Assert.AreNotEqual(notExpected, actual);
        }

        public static void ShouldNotMatch(this string value, Regex pattern)
        {
            StringAssert.DoesNotMatch(value, pattern);
        }

        public static void ShouldStartWith(this string value, string substring)
        {
            StringAssert.StartsWith(value, substring);
        }

        public static IActionSpecification<T> ShouldThrow<T>(this T value, Type exception)
        {
            return new ActionSpecification<T>(value, e =>
            {
                e.ShouldNotBeNull();
                e.ShouldBeInstanceOfType(exception);
            });
        }
    }
}