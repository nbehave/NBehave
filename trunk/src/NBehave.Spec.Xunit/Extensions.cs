using System;
using System.Collections;
using System.Collections.Generic;
using NBehave.Spec.Framework;
using Xunit;

namespace NBehave.Spec.Xunit
{
    public static class Extensions
    {
        public static void ShouldBeTrue(this bool condition)
        {
            Assert.True(condition);
        }

        public static void ShouldBeFalse(this bool condition)
        {
            Assert.False(condition);
        }

        public static void ShouldEqual<T>(this T actual, T expected)
        {
            Assert.Equal(expected, actual);
        }

        public static void ShouldNotEqual<T>(this T actual, T expected)
        {
            Assert.NotEqual(expected, actual);
        }

        public static void ShouldBeTheSameAs(this object actual, object expected)
        {
            Assert.Same(expected, actual);
        }

        public static void ShouldNotBeTheSameAs(this object actual, object expected)
        {
            Assert.NotSame(expected, actual);
        }

        public static T ShouldBeInstanceOf<T>(this object value)
        {
            Assert.IsType<T>(value);
            return (T) value;
        }

        public static void ShouldBeInstanceOf(this object value, Type expectedType)
        {
            Assert.IsType(expectedType, value);
        }

        public static void ShouldNotBeInstanceOf<T>(this object value)
        {
            Assert.IsNotType<T>(value);
        }

        public static void ShouldNotBeInstanceOf(this object value, Type expectedType)
        {
            Assert.IsNotType(expectedType, value);
        }

        public static void ShouldBeNull(this object value)
        {
            Assert.Null(value);
        }

        public static void ShouldNotBeNull(this object value)
        {
            Assert.NotNull(value);
        }

        public static void ShouldContain(this string actual, string expected)
        {
            Assert.Contains(expected, actual);
        }

        public static void ShouldNotContain(this string actual, string expected)
        {
            Assert.DoesNotContain(expected, actual);
        }

        public static void ShouldContain<T>(this IEnumerable<T> collection, T expected)
        {
            Assert.Contains(expected, collection);
        }

        public static void ShouldNotContain<T>(this IEnumerable<T> collection, T notExpected)
        {
            Assert.DoesNotContain(notExpected, collection);
        }

        public static void ShouldBeEmpty(this IEnumerable collection)
        {
            Assert.Empty(collection);
        }

        public static void ShouldNotBeEmpty(this IEnumerable collection)
        {
            Assert.NotEmpty(collection);
        }

        public static void ShouldBeBetween<T>(this T actual, T low, T high)
        {
            Assert.InRange(actual, low, high);
        }

        public static void ShouldNotBeBetween<T>(this T actual, T low, T high)
        {
            Assert.NotInRange(actual, low, high);
        }

        public static void ShouldBeThrownBy(this Type exceptionType, Action expressionThatThrows)
        {
            Assert.ThrowsDelegate d = () => expressionThatThrows();
            Assert.Throws(exceptionType, d);
        }

        public static IActionSpecification<T> ShouldThrow<T>(this T value, Type exception)
        {
            return new ActionSpecification<T>(value, exception);
        }

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

    }
}