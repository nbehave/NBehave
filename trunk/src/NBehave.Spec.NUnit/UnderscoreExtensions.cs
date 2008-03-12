using System;
using System.Collections;

namespace NBehave.Spec.NUnit.Underscore
{
    public static class Extensions
    {
        public static void should_be_true(this bool condition)
        {
            condition.ShouldBeTrue();
        }

        public static void should_be_false(this bool condition)
        {
            condition.ShouldBeFalse();
        }

        public static void should_equal(this object actual, object expected)
        {
            actual.ShouldEqual(expected);
        }

        public static void should_not_equal(this object actual, object expected)
        {
            actual.ShouldNotEqual(expected);
        }

        public static void should_be_the_same_as(this object actual, object expected)
        {
            actual.ShouldBeTheSameAs(expected);
        }

        public static void should_not_be_the_same_as(this object actual, object expected)
        {
            actual.ShouldNotBeTheSameAs(expected);
        }

        public static void should_contain(this ICollection actual, object expected)
        {
            actual.ShouldContain(expected);
        }

        public static void should_be_greater_than(this IComparable arg1, IComparable arg2)
        {
            arg1.ShouldBeGreaterThan(arg2);
        }

        public static void should_be_greater_than_or_equal_to(this IComparable arg1, IComparable arg2)
        {
            arg1.ShouldBeGreaterThanOrEqualTo(arg2);
        }

        public static void should_be_less_than(this IComparable arg1, IComparable arg2)
        {
            arg1.ShouldBeLessThan(arg2);
        }

        public static void should_be_less_than_or_equal_to(this IComparable arg1, IComparable arg2)
        {
            arg1.ShouldBeLessThanOrEqualTo(arg2);
        }

        public static void should_be_assignable_from(this object actual, Type expected)
        {
            actual.ShouldBeAssignableFrom(expected);
        }

        public static void should_be_assignable_from<ExpectedType>(this Object actual)
        {
            actual.ShouldBeAssignableFrom(typeof(ExpectedType));
        }

        public static void should_not_be_assignable_from(this object actual, Type expected)
        {
            actual.ShouldNotBeAssignableFrom(expected);
        }

        public static void should_not_be_assignable_from<ExpectedType>(this object actual)
        {
            actual.ShouldNotBeAssignableFrom(typeof(ExpectedType));
        }

        public static void should_be_empty(this string value)
        {
            value.ShouldBeEmpty();
        }

        public static void should_not_be_empty(this string value)
        {
            value.ShouldNotBeEmpty();
        }

        public static void should_be_empty(this ICollection collection)
        {
            collection.ShouldBeEmpty();
        }

        public static void should_not_be_empty(this ICollection collection)
        {
            collection.ShouldNotBeEmpty();
        }

        public static void should_be_instance_of_type(this object actual, Type expected)
        {
            actual.ShouldBeInstanceOfType(expected);
        }

        public static void should_be_instance_of<ExpectedType>(this object actual)
        {
            actual.ShouldBeInstanceOfType(typeof(ExpectedType));
        }

        public static void should_not_be_instance_of_type(this object actual, Type expected)
        {
            actual.ShouldNotBeInstanceOfType(expected);
        }

        public static void should_not_be_instance_of<ExpectedType>(this object actual)
        {
            actual.ShouldNotBeInstanceOfType(typeof(ExpectedType));
        }

        public static void should_be_NaN(this double value)
        {
            value.ShouldBeNaN();
        }

        public static void should_be_null(this object value)
        {
            value.ShouldBeNull();
        }

        public static void should_not_be_null(this object value)
        {
            value.ShouldNotBeNull();
        }

        public static void should_be_thrown_by(this Type exceptionType, Action action)
        {
            exceptionType.ShouldBeThrownBy(action);
        }
    }
}