using System;
using System.Collections;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NBehave.Spec.MSTest
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

        public static void ShouldEqual<T>(this T actual, T expected)
        {
            Assert.AreEqual(expected, actual);
        }

        public static void ShouldNotEqual<T>(this T actual, T notExpected)
        {
            Assert.AreNotEqual(notExpected, actual);
        }

        public static void ShouldBeTheSameAs(this object actual, object expected)
        {
            Assert.AreSame(expected, actual);
        }

        public static void ShouldNotBeTheSameAs(this object actual, object notExpected)
        {
            Assert.AreNotSame(notExpected, actual);
        }

        public static void ShouldBeInstanceOf(this object value, Type expectedType)
        {
            Assert.IsInstanceOfType(value, expectedType);
        }

        public static void ShouldBeInstanceOf<T>(this object value)
        {
            value.ShouldBeInstanceOf(typeof(T));
        }

        public static void ShouldNotBeInstanceOf(this object value, Type wrongType)
        {
            Assert.IsNotInstanceOfType(value, wrongType);
        }

        public static void ShouldNotBeInstanceOf<T>(this object value)
        {
            value.ShouldNotBeInstanceOf(typeof(T));
        }

        public static void ShouldBeNull(this object value)
        {
            Assert.IsNull(value);
        }

        public static void ShouldNotBeNull(this object value)
        {
            Assert.IsNotNull(value);
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
            e.ShouldBeInstanceOf(exceptionType);
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

        public static void ShouldContain(this string value, string substring)
        {
            StringAssert.Contains(value, substring);
        }

        public static void ShouldStartWith(this string value, string substring)
        {
            StringAssert.StartsWith(value, substring);
        }

        public static void ShouldEndWith(this string value, string substring)
        {
            StringAssert.EndsWith(value, substring);
        }

        public static void ShouldMatch(this string value, Regex pattern)
        {
            StringAssert.Matches(value, pattern);
        }

        public static void ShouldNotMatch(this string value, Regex pattern)
        {
            StringAssert.DoesNotMatch(value, pattern);
        }

        public static void ShouldContain(this ICollection collection, object element)
        {
            CollectionAssert.Contains(collection, element);
        }

        public static void ShouldNotContain(this ICollection collection, object element)
        {
            CollectionAssert.DoesNotContain(collection, element);
        }

        public static void ShouldBeEquivalentTo(this ICollection actual, ICollection expected)
        {
            CollectionAssert.AreEquivalent(expected, actual);
        }

        public static void ShouldNotBeEquivalentTo(this ICollection actual, ICollection expected)
        {
            CollectionAssert.AreNotEquivalent(expected, actual);
        }

        public static void ShouldBeEqualTo(this ICollection actual, ICollection expected)
        {
            CollectionAssert.AreEqual(expected, actual);
        }

        public static void ShouldNotBeEqualTo(this ICollection actual, ICollection expected)
        {
            CollectionAssert.AreNotEqual(expected, actual);
        }
    }
}