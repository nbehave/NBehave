using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Xunit;

namespace NBehave.Spec.Xunit
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
                throw new ArgumentException("type (T) must be float, double or decimal");

            if (typeof(T) == typeof(decimal))
            {
                double low = Decimal.ToDouble(Convert.ToDecimal(actual)) - Decimal.ToDouble(Convert.ToDecimal(delta));
                double high = Decimal.ToDouble(Convert.ToDecimal(actual)) + Decimal.ToDouble(Convert.ToDecimal(delta));
                Assert.InRange(Decimal.ToDouble(Convert.ToDecimal(expected)),
                               low,
                               high);
            }
            if (typeof(T) == typeof(double))
            {
                double low = Convert.ToDouble(actual) - Convert.ToDouble(delta);
                double high = Convert.ToDouble(actual) + Convert.ToDouble(delta);
                Assert.InRange(Convert.ToDouble(expected), low, high);
            }
            if (typeof(T) == typeof(float))
            {
                float low = Convert.ToSingle(actual) - Convert.ToSingle(delta);
                float high = Convert.ToSingle(actual) + Convert.ToSingle(delta);
                Assert.InRange(Convert.ToSingle(expected), low, high);
            }
        }

        public static void ShouldBeAssignableFrom<TExpectedType>(this Object actual)
        {
            Assert.IsAssignableFrom(typeof(TExpectedType), actual);
        }

        public static void ShouldBeAssignableFrom(this object actual, Type expected)
        {
            Assert.IsAssignableFrom(expected, actual);
        }

        public static void ShouldBeEmpty(this string value)
        {
            Assert.Empty(value);
        }

        public static void ShouldBeEmpty(this IEnumerable collection)
        {
            Assert.Empty(collection);
        }

        public static void ShouldBeEqualTo(this ICollection actual, ICollection expected)
        {
            Assert.Equal(expected.Count, actual.Count);
            var actualList = new ArrayList(actual);
            var expectedList = new ArrayList(expected);
            for (int i = 0; i < actualList.Count; i++)
                Assert.Equal(expectedList[i], actualList[i]);
        }

        public static void ShouldBeEquivalentTo(this ICollection actual, ICollection expected)
        {
            Assert.Equal(expected.Count, actual.Count);
            var expectedList = new ArrayList(expected);
            foreach (var actualItem in actual)
                Assert.True(expectedList.Contains(actualItem));
        }

        public static void ShouldBeFalse(this bool condition)
        {
            Assert.False(condition);
        }

        public static void ShouldBeGreaterThan(this IComparable arg1, IComparable arg2)
        {
            Assert.True(arg1.CompareTo(arg2) == 1, string.Format("Expected {0} > {1}", arg1, arg2));
        }

        public static void ShouldBeGreaterThanOrEqualTo(this IComparable arg1, IComparable arg2)
        {
            Assert.True(arg1.CompareTo(arg2) >= 0, string.Format("Expected {0} >= {1}", arg1, arg2));
        }

        public static void ShouldBeInstanceOfType<TExpectedType>(this object actual)
        {
            actual.ShouldBeInstanceOfType(typeof(TExpectedType));
        }

        public static void ShouldBeInstanceOfType(this object actual, Type expected)
        {
            Assert.IsType(expected, actual);
        }

        public static void ShouldBeLessThan(this IComparable arg1, IComparable arg2)
        {
            Assert.True(arg1.CompareTo(arg2) == -1, string.Format("Expected {0} < {1}", arg1, arg2));
        }

        public static void ShouldBeLessThanOrEqualTo(this IComparable arg1, IComparable arg2)
        {
            Assert.True(arg1.CompareTo(arg2) <= 0, string.Format("Expected {0} <= {1}", arg1, arg2));
        }

        public static void ShouldBeNaN(this double value)
        {
            Assert.True(double.IsNaN(value), "Expected value to be NaN");
        }

        public static void ShouldBeNull(this object value)
        {
            Assert.Null(value);
        }

        public static void ShouldBeTheSameAs<T>(this T actual, T expected) where T : class
        {
            Assert.Same(expected, actual);
        }

        public static void ShouldBeThrownBy(this Type exceptionType, ThrowingAction action)
        {
            Action a = action.Invoke;
            Assert.Throws(exceptionType, a);
        }

        public static void ShouldBeTrue(this bool condition)
        {
            Assert.True(condition);
        }

        public static void ShouldContain(this string actual, string expected)
        {
            Assert.Contains(expected, actual);
        }

        public static void ShouldContain<T>(this IEnumerable<T> actual, T expected)
        {
            Assert.Contains(expected, actual);
        }

        public static void ShouldEndWith(this string value, string substring)
        {
            Assert.True((value.EndsWith(substring)), string.Format("Expected '{0}' to end with '{1}", value, substring));
        }

        public static void ShouldEqual<T>(this T actual, T expected)
        {
            Assert.Equal(expected, actual);
        }

        public static void ShouldMatch(this string value, Regex pattern)
        {
            Assert.True(pattern.IsMatch(value), string.Format("string \"{0}\" does not match pattern {1}", value, pattern));
        }

        public static void ShouldMatch(this string actual, string regexPattern, RegexOptions regexOptions)
        {
            var r = new Regex(regexPattern, regexOptions);
            ShouldMatch(actual, r);
        }

        public static void ShouldNotBeAssignableFrom<TExpectedType>(this object actual)
        {
            actual.ShouldNotBeAssignableFrom(typeof(TExpectedType));
        }

        public static void ShouldNotBeAssignableFrom(this object actual, Type expected)
        {
            try
            {
                Assert.IsAssignableFrom(expected, actual);
                Assert.False(true, string.Format("Object of type {0} should not be assignable from type '{1}'", actual.GetType(), expected));
            }
            catch (Exception)
            { }
        }

        public static void ShouldNotBeEmpty(this string value)
        {
            Assert.NotEmpty(value);
        }

        public static void ShouldNotBeEmpty(this ICollection collection)
        {
            Assert.NotEmpty(collection);
        }

        public static void ShouldNotBeEqualTo(this ICollection actual, ICollection expected)
        {
            var actualList = new ArrayList(actual);
            var expectedList = new ArrayList(expected);
            bool areEqual = true;
            for (int i = 0; i < actualList.Count && areEqual; i++)
                areEqual = expectedList[i] == actualList[i];
            areEqual = areEqual && expected.Count == actual.Count;
            Assert.False(areEqual, "collections are equal");
        }

        public static void ShouldNotBeEquivalentTo(this ICollection actual, ICollection expected)
        {
            bool areEqual = true;
            var expectedList = new ArrayList(expected);
            foreach (var actualItem in actual)
                areEqual = areEqual && expectedList.Contains(actualItem);
            areEqual = areEqual && expected.Count == actual.Count;
            Assert.False(areEqual, "collections are equivalent to each other");
        }

        public static void ShouldNotBeInstanceOfType<TExpectedType>(this object actual)
        {
            actual.ShouldNotBeInstanceOfType(typeof(TExpectedType));
        }

        public static void ShouldNotBeInstanceOfType(this object actual, Type expected)
        {
            Assert.IsNotType(expected, actual);
        }

        public static void ShouldNotBeNull(this object value)
        {
            Assert.NotNull(value);
        }

        public static void ShouldNotBeTheSameAs(this object actual, object notExpected)
        {
            Assert.NotSame(notExpected, actual);
        }

        public static void ShouldNotContain<T>(this IEnumerable<T> list, T notExpected)
        {
            Assert.DoesNotContain(notExpected, list);
        }

        public static void ShouldNotContain(this string actual, string notExpected)
        {
            Assert.DoesNotContain(notExpected, actual);
        }

        public static void ShouldNotEqual<T>(this T actual, T notExpected)
        {
            Assert.NotEqual(notExpected, actual);
        }

        public static void ShouldNotMatch(this string value, Regex pattern)
        {
            Assert.True(pattern.IsMatch(value) == false, string.Format("string \"{0}\" should not match pattern {1}", value, pattern));
        }

        public static void ShouldStartWith(this string value, string substring)
        {
            Assert.True((value.StartsWith(substring)), string.Format("Expected '{0}' to start with '{1}", value, substring));
        }

        public static IActionSpecification<T> ShouldThrow<T>(this T value, Type exception)
        {
            return new ActionSpecification<T>(value, e =>
                                                         {
                                                             e.ShouldNotBeNull();
                                                             e.ShouldBeInstanceOfType(exception);
                                                         });
        }

        public static Exception ShouldThrow<T>(this Action action)
        {
            var failed = false;
            var ex = new Exception("");
            try
            {
                action();
                failed = true;
            }
            catch (Exception e)
            {
                e.ShouldBeInstanceOfType(typeof(T));
                ex = e;
            }
            if (failed)
                Assert.False(failed, string.Format("Exception of type <{0}> expected but no exception occurred", typeof(T)));
            return ex;
        }

        public static void WithExceptionMessage(this Exception e, string exceptionMessage)
        {
            exceptionMessage.ShouldEqual(e.Message);
        }
    }
}
