using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using MbUnit.Framework;

namespace NBehave.Spec.MbUnit
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
            Assert.AreApproximatelyEqual(expected, actual, delta);
        }

        public static void ShouldBeAssignableFrom<TExpectedType>(this Object actual)
        {
            actual.ShouldBeAssignableFrom(typeof(TExpectedType));
        }

        public static void ShouldBeAssignableFrom(this object actual, Type expected)
        {
            Assert.IsAssignableFrom(expected, actual);
        }

        public static void ShouldBeEmpty(this string value)
        {
            Assert.IsEmpty(value);
        }

        public static void ShouldBeEmpty(this IEnumerable collection)
        {
            Assert.IsEmpty(collection);
        }

        public static void ShouldBeEqualTo<T>(this IEnumerable<T> actual, IEnumerable<T> expected)
        {
            Assert.AreEqual(expected, actual);
        }

        public static void ShouldBeEquivalentTo<T>(this IEnumerable<T> actual, IEnumerable<T> expected)
        {
            Assert.AreElementsEqualIgnoringOrder(expected, actual);
        }

        public static void ShouldBeFalse(this bool condition)
        {
            Assert.IsFalse(condition);
        }

        public static void ShouldBeGreaterThan(this IComparable arg1, IComparable arg2)
        {
            Assert.GreaterThan(arg1, arg2);
        }

        public static void ShouldBeGreaterThanOrEqualTo(this IComparable arg1, IComparable arg2)
        {
            Assert.GreaterThanOrEqualTo(arg1, arg2);
        }

        [Obsolete("Use ShouldBeInstanceOfType")]
        public static void ShouldBeInstanceOf<TExpectedType>(this object actual)
        {
            actual.ShouldBeInstanceOfType(typeof(TExpectedType));
        }

        public static void ShouldBeInstanceOfType<TExpectedType>(this object actual)
        {
            actual.ShouldBeInstanceOfType(typeof(TExpectedType));
        }

        public static void ShouldBeInstanceOfType(this object actual, Type expected)
        {
            Assert.IsInstanceOfType(expected, actual);
        }

        public static void ShouldBeLessThan(this IComparable arg1, IComparable arg2)
        {
            Assert.LessThan(arg1, arg2);
        }

        public static void ShouldBeLessThanOrEqualTo(this IComparable arg1, IComparable arg2)
        {
            Assert.LessThanOrEqualTo(arg1, arg2);
        }

        public static void ShouldBeNaN(this double value)
        {
            Assert.AreEqual(value, double.NaN);
        }

        public static void ShouldBeNull(this object value)
        {
            Assert.IsNull(value);
        }

        public static void ShouldBeTheSameAs<T>(this T actual, T expected) where T : class
        {
            Assert.AreSame(actual, expected);
        }

        public static void ShouldBeThrownBy(this Type exceptionType, ThrowingAction action)
        {
        	Assert.Throws(exceptionType, action.Invoke);
        }

        public static void ShouldBeTrue(this bool condition)
        {
            Assert.IsTrue(condition);
        }

        public static void ShouldContain<T>(this IEnumerable<T> actual, T expected)
        {
            Assert.Contains(actual, expected);
        }

        public static void ShouldContain(this string actual, string expected)
        {
            Assert.Contains(actual, expected);
        }

        public static void ShouldEndWith(this string actual, string expected)
        {
            Assert.EndsWith(actual, expected);
        }

        public static void ShouldEqual<T>(this T actual, T expected)
        {
            Assert.AreEqual(actual, expected);
        }

        public static void ShouldFullyMatch(this string actual, string regExPattern)
        {
            Assert.FullMatch(actual, regExPattern);
        }

        public static void ShouldFullyMatch(this string actual, string regexPattern, RegexOptions regexOptions)
        {
            Assert.FullMatch(actual, regexPattern, regexOptions);
        }

        public static void ShouldMatch(this string actual, string regexPattern)
        {
            Assert.Like(actual, regexPattern);
        }

        public static void ShouldMatch(this string actual, string regexPattern, RegexOptions regexOptions)
        {
            Assert.Like(actual, regexPattern, regexOptions);
        }

        public static void ShouldNotApproximatelyEqual<T>(this T actual, T expected, T delta)
        {
            Assert.AreNotApproximatelyEqual(expected, actual, delta);
        }

        public static void ShouldNotBeAssignableFrom<TExpectedType>(this object actual)
        {
            actual.ShouldNotBeAssignableFrom(typeof(TExpectedType));
        }

        public static void ShouldNotBeAssignableFrom(this object actual, Type expected)
        {
            Assert.IsNotAssignableFrom(expected, actual);
        }

        public static void ShouldNotBeEmpty(this string value)
        {
            Assert.IsNotEmpty(value);
        }

        public static void ShouldNotBeEmpty(this IEnumerable collection)
        {
            Assert.IsNotEmpty(collection);
        }

        public static void ShouldNotBeEqualTo<T>(this IEnumerable<T> actual, IEnumerable<T> expected)
        {
            Assert.AreNotEqual(expected, actual);
        }

        public static void ShouldNotBeEquivalentTo<T>(this IEnumerable<T> actual, IEnumerable<T> expected)
        {
            Assert.AreElementsNotEqual(expected, actual);
        }

        [Obsolete("Use ShouldNotBeInstanceOfType")]
        public static void ShouldNotBeInstanceOf<TExpectedType>(this object actual)
        {
            actual.ShouldNotBeInstanceOfType(typeof(TExpectedType));
        }

        public static void ShouldNotBeInstanceOfType<TExpectedType>(this object actual)
        {
            actual.ShouldNotBeInstanceOfType(typeof(TExpectedType));
        }

        public static void ShouldNotBeInstanceOfType(this object actual, Type expected)
        {
            Assert.IsNotInstanceOfType(expected, actual);
        }

        public static void ShouldNotBeNull(this object value)
        {
            Assert.IsNotNull(value);
        }

        public static void ShouldNotBeTheSameAs<T>(this T actual, T expected) where T : class
        {
            Assert.AreNotSame(actual, expected);
        }

        public static void ShouldNotContain<T>(this IEnumerable<T> actual, T expected)
        {
            Assert.DoesNotContain(actual, expected);
        }

        public static void ShouldNotContain(this string actual, string expected)
        {
            Assert.DoesNotContain(actual, expected);
        }

        public static void ShouldNotEqual<T>(this T actual, T expected)
        {
            Assert.AreNotEqual(actual, expected);
        }

        public static void ShouldNotMatch(this string value, Regex pattern)
        {
            Assert.IsTrue(pattern.IsMatch(value) == false, string.Format("string \"{0}\" should not match pattern {1}", value, pattern));
        }

        public static void ShouldStartWith(this string actual, string expected)
        {
            Assert.StartsWith(actual, expected);
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
