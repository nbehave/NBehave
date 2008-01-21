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
		
		public static void should_be_true(this bool condition)
		{
			condition.ShouldBeTrue();
		}

        public static void ShouldBeFalse(this bool condition)
        {
            Assert.That(condition, Is.False);
        }

		public static void should_be_false(this bool condition)
		{
			condition.ShouldBeFalse();
		}

        public static void ShouldEqual(this object actual, object expected)
        {
            Assert.That(actual, Is.EqualTo(expected));
        }

		public static void should_equal(this object actual, object expected)
		{
			actual.ShouldEqual(expected);
		}

        public static void ShouldNotEqual(this object actual, object expected)
        {
            Assert.That(actual, Is.Not.EqualTo(expected));
        }

		public static void should_not_equal(this object actual, object expected)
		{
			actual.ShouldNotEqual(expected);
		}

        public static void ShouldBeTheSameAs(this object actual, object expected)
        {
            Assert.That(actual, Is.SameAs(expected));
        }

		public static void should_be_the_same_as(this object actual, object expected)
		{
			actual.ShouldBeTheSameAs(expected);
		}

        public static void ShouldNotBeTheSameAs(this object actual, object expected)
        {
            Assert.That(actual, Is.Not.SameAs(expected));
        }

		public static void should_not_be_the_same_as(this object actual, object expected)
		{
			actual.ShouldNotBeTheSameAs(expected);
		}

        public static void ShouldContain(this ICollection actual, object expected)
        {
            Assert.Contains(expected, actual);
        }

		public static void should_contain(this ICollection actual, object expected)
		{
			actual.ShouldContain(expected);
		}

        public static void ShouldBeGreaterThan(this IComparable arg1, IComparable arg2)
        {
            Assert.That(arg1, Is.GreaterThan(arg2));
        }

		public static void should_be_greater_than(this IComparable arg1, IComparable arg2)
		{
			arg1.ShouldBeGreaterThan(arg2);
		}
    
        public static void ShouldBeGreaterThanOrEqualTo(this IComparable arg1, IComparable arg2)
        {
            Assert.That(arg1, Is.GreaterThanOrEqualTo(arg2));
        }

		public static void should_be_greater_than_or_equal_to(this IComparable arg1, IComparable arg2)
		{
			arg1.ShouldBeGreaterThanOrEqualTo(arg2);
		}

        public static void ShouldBeLessThan(this IComparable arg1, IComparable arg2)
        {
            Assert.That(arg1, Is.LessThan(arg2));
        }

		public static void should_be_less_than(this IComparable arg1, IComparable arg2)
		{
			arg1.ShouldBeLessThan(arg2);
		}

        public static void ShouldBeLessThanOrEqualTo(this IComparable arg1, IComparable arg2)
        {
            Assert.That(arg1, Is.LessThanOrEqualTo(arg2));
        }

		public static void should_be_less_than_or_equal_to(this IComparable arg1, IComparable arg2)
		{
			Assert.That(arg1, Is.LessThanOrEqualTo(arg2));
		}

        public static void ShouldBeAssignableFrom(this object actual, Type expected)
        {
            Assert.That(actual, Is.AssignableFrom(expected));
        }

		public static void should_be_assignable_from(this object actual, Type expected)		
		{
			Assert.That(actual, Is.AssignableFrom(expected));
		}

        public static void ShouldBeAssignableFrom<ExpectedType>(this Object actual)
        {
            actual.ShouldBeAssignableFrom(typeof (ExpectedType));
        }

		public static void should_be_assignable_from<ExpectedType>(this Object actual)
		{
			actual.ShouldBeAssignableFrom(typeof(ExpectedType));
		}

        public static void ShouldNotBeAssignableFrom(this object actual, Type expected)
        {
            Assert.That(actual, Is.Not.AssignableFrom(expected));
        }

		public static void should_not_be_assignable_from(this object actual, Type expected)
		{
			Assert.That(actual, Is.Not.AssignableFrom(expected));
		}

        public static void ShouldNotBeAssignableFrom<ExpectedType>(this object actual)
        {
            actual.ShouldNotBeAssignableFrom(typeof (ExpectedType));
        }

		public static void should_not_be_assignable_from<ExpectedType>(this object actual)
		{
			actual.ShouldNotBeAssignableFrom(typeof(ExpectedType));
		}

        public static void ShouldBeEmpty(this string value)
        {
            Assert.That(value, Is.Empty);
        }

		public static void should_be_empty(this string value)
		{
			Assert.That(value, Is.Empty);
		}

        public static void ShouldNotBeEmpty(this string value)
        {
            Assert.That(value, Is.Not.Empty);
        }

		public static void should_not_be_empty(this string value)
		{
			Assert.That(value, Is.Not.Empty);
		}

        public static void ShouldBeEmpty(this ICollection collection)
        {
            Assert.That(collection, Is.Empty);
        }

		public static void should_be_empty(this ICollection collection)
		{
			Assert.That(collection, Is.Empty);
		}

        public static void ShouldNotBeEmpty(this ICollection collection)
        {
            Assert.That(collection, Is.Not.Empty);
        }

		public static void should_not_be_empty(this ICollection collection)
		{
			Assert.That(collection, Is.Not.Empty);
		}

        public static void ShouldBeInstanceOfType(this object actual, Type expected)
        {
            Assert.That(actual, Is.InstanceOfType(expected));
        }

		public static void should_be_instance_of_type(this object actual, Type expected)
		{
			Assert.That(actual, Is.InstanceOfType(expected));
		}

        public static void ShouldBeInstanceOf<ExpectedType>(this object actual)
        {
            actual.ShouldBeInstanceOfType(typeof(ExpectedType));
        }

		public static void should_be_instance_of<ExpectedType>(this object actual)
		{
			actual.ShouldBeInstanceOfType(typeof(ExpectedType));
		}

        public static void ShouldNotBeInstanceOfType(this object actual, Type expected)
        {
            Assert.That(actual, Is.Not.InstanceOfType(expected));
        }

		public static void should_not_be_instance_of_type(this object actual, Type expected)
		{
			Assert.That(actual, Is.Not.InstanceOfType(expected));
		}

        public static void ShouldNotBeInstanceOf<ExpectedType>(this object actual)
        {
            actual.ShouldNotBeInstanceOfType(typeof (ExpectedType));
        }

		public static void should_not_be_instance_of<ExpectedType>(this object actual)
		{
			actual.ShouldNotBeInstanceOfType(typeof(ExpectedType));
		}

        public static void ShouldBeNaN(this double value)
        {
            Assert.That(value, Is.NaN);
        }

		public static void should_be_NaN(this double value)
		{
			Assert.That(value, Is.NaN);
		}

        public static void ShouldBeNull(this object value)
        {
            Assert.That(value, Is.Null);
        }

		public static void should_be_null(this object value)
		{
			Assert.That(value, Is.Null);
		}

        public static void ShouldNotBeNull(this object value)
        {
            Assert.That(value, Is.Not.Null);
        }

		public static void should_not_be_null(this object value)
		{
			Assert.That(value, Is.Not.Null);
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

		public static void should_be_thrown_by(this Type exceptionType, Action action)
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
