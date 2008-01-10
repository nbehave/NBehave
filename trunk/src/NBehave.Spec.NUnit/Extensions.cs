using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using NUnit.Framework.SyntaxHelpers;
using System.Collections;

namespace NBehave.Spec.NUnit
{
    public static class Extensions
    {
        public static void ShouldBeTrue(this bool condition)
        {
            Assert.That(condition, Is.True);
        }

        public static void ShouldBeFalse(this bool condition)
        {
            Assert.That(condition, Is.False);
        }

        public static void ShouldEqual(this object actual, object expected)
        {
            Assert.That(actual, Is.EqualTo(expected));
        }

        public static void ShouldNotEqual(this object actual, object expected)
        {
            Assert.That(actual, Is.Not.EqualTo(expected));
        }

        public static void ShouldBeTheSameAs(this object actual, object expected)
        {
            Assert.That(actual, Is.SameAs(expected));
        }

        public static void ShouldNotBeTheSameAs(this object actual, object expected)
        {
            Assert.That(actual, Is.Not.SameAs(expected));
        }

        public static void ShouldContain(this ICollection actual, object expected)
        {
            Assert.Contains(expected, actual);
        }

        public static void ShouldBeGreaterThan(this IComparable arg1, IComparable arg2)
        {
            Assert.That(arg1, Is.GreaterThan(arg2));
        }
    
        public static void ShouldBeGreaterThanOrEqualTo(this IComparable arg1, IComparable arg2)
        {
            Assert.That(arg1, Is.GreaterThanOrEqualTo(arg2));
        }

        public static void ShouldBeLessThan(this IComparable arg1, IComparable arg2)
        {
            Assert.That(arg1, Is.LessThan(arg2));
        }

        public static void ShouldBeLessThanOrEqualTo(this IComparable arg1, IComparable arg2)
        {
            Assert.That(arg1, Is.LessThanOrEqualTo(arg2));
        }

        public static void ShouldBeAssignableFrom(this object actual, Type expected)
        {
            Assert.That(actual, Is.AssignableFrom(expected));
        }

        public static void ShouldNotBeAssignableFrom(this object actual, Type expected)
        {
            Assert.That(actual, Is.Not.AssignableFrom(expected));
        }

        public static void ShouldBeEmpty(this string value)
        {
            Assert.That(value, Is.Empty);
        }

        public static void ShouldNotBeEmpty(this string value)
        {
            Assert.That(value, Is.Not.Empty);
        }

        public static void ShouldBeEmpty(this ICollection collection)
        {
            Assert.That(collection, Is.Empty);
        }

        public static void ShouldNotBeEmpty(this ICollection collection)
        {
            Assert.That(collection, Is.Not.Empty);
        }

        public static void ShouldBeInstanceOfType(this object actual, Type expected)
        {
            Assert.That(actual, Is.InstanceOfType(expected));
        }

        public static void ShouldNotBeInstanceOfType(this object actual, Type expected)
        {
            Assert.That(actual, Is.Not.InstanceOfType(expected));
        }

        public static void ShouldBeNaN(this double value)
        {
            Assert.That(value, Is.NaN);
        }

        public static void ShouldBeNull(this object value)
        {
            Assert.That(value, Is.Null);
        }

        public static void ShouldNotBeNull(this object value)
        {
            Assert.That(value, Is.Not.Null);
        }


    }
}
