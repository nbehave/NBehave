using System;
using System.Text.RegularExpressions;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using System.Collections;

namespace NBehave.Spec.NUnit
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

        public static void ShouldBeAssignableFrom<TExpectedType>(this Object actual)
        {
            Assert.IsAssignableFrom(typeof(TExpectedType), actual);
        }

        public static void ShouldBeAssignableFrom(this object actual, Type expected)
        {
            Assert.That(actual, Is.AssignableFrom(expected));
        }

        public static void ShouldBeEmpty(this string value)
        {
            Assert.That(value, Is.Empty);
        }

        public static void ShouldBeEmpty(this IEnumerable collection)
        {
            Assert.That(collection, Is.Empty);
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
            Assert.That(condition, Is.False);
        }

        public static void ShouldBeGreaterThan(this IComparable arg1, IComparable arg2)
        {
            Assert.That(arg1, Is.GreaterThan(arg2));
        }

        public static void ShouldBeGreaterThanOrEqualTo(this IComparable arg1, IComparable arg2)
        {
            Assert.That(arg1, Is.GreaterThanOrEqualTo(arg2));
        }

        public static void ShouldBeInstanceOf<TExpectedType>(this object actual)
        {
            actual.ShouldBeInstanceOf(typeof(TExpectedType));
        }

        public static void ShouldBeInstanceOf(this object actual, Type expected)
        {
            Assert.That(actual, Is.InstanceOfType(expected));
        }

        [Obsolete("Use ShouldBeInstanceOf")]
        public static void ShouldBeInstanceOfType(this object actual, Type expected)
        {
            Assert.That(actual, Is.InstanceOfType(expected));
        }

        public static void ShouldBeLessThan(this IComparable arg1, IComparable arg2)
        {
            Assert.That(arg1, Is.LessThan(arg2));
        }

        public static void ShouldBeLessThanOrEqualTo(this IComparable arg1, IComparable arg2)
        {
            Assert.That(arg1, Is.LessThanOrEqualTo(arg2));
        }

        public static void ShouldBeNaN(this double value)
        {
            Assert.That(value, Is.NaN);
        }

        public static void ShouldBeNull(this object value)
        {
            Assert.That(value, Is.Null);
        }

        public static void ShouldBeTheSameAs(this object actual, object expected)
        {
            Assert.That(actual, Is.SameAs(expected));
        }

        public static void ShouldBeThrownBy(this Type exceptionType, ThrowingAction action)
        {
            Exception e = null;

            try
            {
                action.Invoke();
            }
            catch (Exception ex)
            {
                e = ex;
            }

            e.ShouldNotBeNull();
            e.ShouldBeInstanceOf(exceptionType);

        }

        public static void ShouldBeTrue(this bool condition)
        {
            Assert.That(condition, Is.True);
        }

        public static void ShouldContain(this ICollection actual, object expected)
        {
            Assert.Contains(expected, actual);
        }

        public static void ShouldContain(this IEnumerable actual, object expected)
        {
            var lst = new ArrayList();
            foreach (var o in actual)
            {
                lst.Add(o);
            }
            ShouldContain(lst, expected);
        }

        public static void ShouldEndWith(this string value, string substring)
        {
            StringAssert.EndsWith(substring, value);
        }

        public static void ShouldEqual<T>(this T actual, T expected)
        {
            Assert.That(actual, Is.EqualTo(expected));
        }

        public static void ShouldMatch(this string value, Regex pattern)
        {
            Assert.IsTrue(pattern.IsMatch(value), string.Format("string \"{0}\" does not match pattern {1}", value, pattern));
        }

        public static void ShouldNotBeAssignableFrom<TExpectedType>(this object actual)
        {
            actual.ShouldNotBeAssignableFrom(typeof(TExpectedType));
        }

        public static void ShouldNotBeAssignableFrom(this object actual, Type expected)
        {
            Assert.That(actual, Is.Not.AssignableFrom(expected));
        }

        public static void ShouldNotBeEmpty(this string value)
        {
            Assert.That(value, Is.Not.Empty);
        }

        public static void ShouldNotBeEmpty(this ICollection collection)
        {
            Assert.That(collection, Is.Not.Empty);
        }

        public static void ShouldNotBeEqualTo(this ICollection actual, ICollection expected)
        {
            CollectionAssert.AreNotEqual(expected, actual);
        }

        public static void ShouldNotBeEquivalentTo(this ICollection actual, ICollection expected)
        {
            CollectionAssert.AreNotEquivalent(expected, actual);
        }

        public static void ShouldNotBeInstanceOf<TExpectedType>(this object actual)
        {
            actual.ShouldNotBeInstanceOf(typeof(TExpectedType));
        }

        public static void ShouldNotBeInstanceOf(this object actual, Type expected)
        {
            Assert.That(actual, Is.Not.InstanceOfType(expected));
        }

        [Obsolete("Use ShouldNotBeInstanceOf")]
        public static void ShouldNotBeInstanceOfType(this object actual, Type expected)
        {
            Assert.That(actual, Is.Not.InstanceOfType(expected));
        }

        public static void ShouldNotBeNull(this object value)
        {
            Assert.That(value, Is.Not.Null);
        }

        public static void ShouldNotBeTheSameAs(this object actual, object notExpected)
        {
            Assert.That(actual, Is.Not.SameAs(notExpected));
        }

        public static void ShouldNotContain(this IEnumerable list, object expected)
        {
            Assert.That(list, Is.Not.Contains(expected));
        }

        public static void ShouldNotEqual<T>(this T actual, T notExpected)
        {
            Assert.That(actual, Is.Not.EqualTo(notExpected));
        }


        public static void ShouldNotMatch(this string value, Regex pattern)
        {
            Assert.IsTrue(pattern.IsMatch(value) == false, string.Format("string \"{0}\" should not match pattern {1}", value, pattern));
        }

        public static void ShouldStartWith(this string value, string substring)
        {
            StringAssert.StartsWith(substring, value);
        }

        public static IActionSpecification<T> ShouldThrow<T>(this T value, Type exception)
        {
            return new ActionSpecification<T>(value, e =>
            {
                e.ShouldNotBeNull();
                e.ShouldBeInstanceOf(exception);
            });
        }
    }
}
