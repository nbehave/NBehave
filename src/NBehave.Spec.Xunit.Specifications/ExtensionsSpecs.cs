using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using NUnit.Framework;
using Xunit.Sdk;
using Context = NUnit.Framework.TestFixtureAttribute;

namespace NBehave.Spec.Xunit.Specs
{
    [TestFixture]
    public class When_using_BDD_style_language_for_boolean_assertions
    {
        [Test]
        public void Should_allow_substitution_for_IsFalse()
        {
            false.ShouldBeFalse();
        }

        [Test]
        public void Should_allow_substitution_for_IsTrue()
        {
            true.ShouldBeTrue();
        }
    }

    [TestFixture]
    public class When_using_BDD_style_language_for_equality_assertions
    {
        [Test]
        public void Should_allow_substitution_for_AreEqual()
        {
            int i, j;
            i = 5;
            j = 5;

            i.ShouldEqual(j);
        }

        [Test]
        public void Should_allow_substitution_for_AreNotEqual()
        {
            var i = 5;
            var j = 6;

            i.ShouldNotEqual(j);
        }

        [Test]
        public void Should_allow_substitution_for_AreNotSame()
        {
            object test1 = "blarg";
            object test2 = "splorg";

            test2.ShouldNotBeTheSameAs(test1);
        }

        [Test]
        public void Should_allow_substitution_for_AreSame()
        {
            object test1 = "blarg";
            var test2 = test1;

            test2.ShouldBeTheSameAs(test1);
        }
    }

    [TestFixture]
    public class When_using_BDD_style_language_for_collection_assertions
    {
        [Test]
        public void Should_allow_substitution_for_Contains_on_collections()
        {
            int[] vals = { 5, 6, 7, 8 };

            vals.ShouldContain(6);
        }

        [Test]
        public void Should_allow_substitution_for_ShouldNotContain_on_collections()
        {
            int[] vals = { 5, 6, 7, 8 };

            vals.ShouldNotContain(1);
        }

        [Test]
        public void Should_allow_substitution_for_IsEmpty_for_collections()
        {
            int[] vals = { };

            vals.ShouldBeEmpty();
        }

        [Test]
        public void Should_allow_substitution_for_IsNotEmpty_for_collections()
        {
            int[] vals = { 1, 2, 3 };

            vals.ShouldNotBeEmpty();
        }

        [Test]
        public void should_allow_substitution_for_contains_on_ienumerable()
        {
            IEnumerable<object> lst = new object[] { 1, 2, 3, 4 };
            lst.ShouldContain(3);
        }

        [Test]
        public void Should_allow_substitutions_for_AreEqual()
        {
            int[] values1 = { 4, 5, 6 };
            int[] values2 = { 4, 5, 6 };

            values1.ShouldBeEqualTo(values2);
        }

        [Test]
        public void Should_allow_substitutions_for_AreNotEqual()
        {
            int[] values1 = { 4, 5, 6 };
            int[] values2 = { 5, 6, 4 };

            values1.ShouldNotBeEqualTo(values2);
        }

        [Test]
        public void Should_allow_substitutions_for_AreEquivalent()
        {
            int[] values1 = { 4, 5, 6 };
            int[] values2 = { 6, 4, 5 };

            values1.ShouldBeEquivalentTo(values2);
        }

        [Test]
        public void Should_allow_substitutions_for_AreNotEquivalent()
        {
            int[] values1 = { 4, 5, 6 };
            int[] values2 = { 6, 4, 7 };

            values1.ShouldNotBeEquivalentTo(values2);
        }
    }

    [TestFixture]
    public class When_using_BDD_style_language_for_integer_assertions
    {
        [Test]
        public void Should_allow_substitution_for_Greater()
        {
            5.ShouldBeGreaterThan(4);
        }

        [Test]
        public void Should_allow_substitution_for_GreaterOrEqual()
        {
            5.ShouldBeGreaterThanOrEqualTo(5);
        }

        [Test]
        public void Should_allow_substitution_for_IsNaN()
        {
            double.NaN.ShouldBeNaN();
        }

        [Test]
        public void Should_allow_substitution_for_Less()
        {
            5.ShouldBeLessThan(6);
        }

        [Test]
        public void Should_allow_substitution_for_LessOrEqualTo()
        {
            5.ShouldBeLessThanOrEqualTo(6);
        }
    }

    [TestFixture]
    public class When_using_BDD_style_language_for_string_assertions
    {
        [Test]
        public void Should_allow_substitutions_for_EndsWith()
        {
            "blarg".ShouldEndWith("arg");
        }

        [Test]
        public void Should_allow_substitutions_for_Matches()
        {
            "blarg".ShouldMatch(new Regex("blarg"));
        }

        [Test]
        public void Should_allow_substitution_for_IsEmpty_for_strings()
        {
            string.Empty.ShouldBeEmpty();
        }

        [Test]
        public void Should_allow_substitution_for_IsNotEmpty_for_strings()
        {
            "blarg".ShouldNotBeEmpty();
        }

        [Test]
        public void Should_allow_substitutions_for_DoesNotMatch()
        {
            "blarg".ShouldNotMatch(new Regex("asdf"));
        }

        [Test]
        public void Should_allow_substitutions_for_StartsWith()
        {
            "blarg".ShouldStartWith("bl");
        }

        [Test]
        public void Should_allow_substitution_for_ShouldNotContain_for_string()
        {
            "Lorem ipsum dolor sit amet.".ShouldNotContain("foo");
        }

        [Test, ExpectedException(typeof(DoesNotContainException))]
        public void Should_allow_substitution_for_ShouldNotContain__for_string_failing()
        {
            "Lorem ipsum dolor sit amet".ShouldNotContain("ipsum");
        }

        [Test]
        public void Should_allow_substitution_for_ShouldContain_for_string()
        {
            var str = "Hello";
            str.ShouldContain("Hell");
        }

        [Test, ExpectedException(typeof(ContainsException))]
        public void Should_allow_substitution_for_ShouldContain_for_string_failing()
        {
            var str = "Hello";
            str.ShouldContain("Foo");
        }
    }

    [TestFixture]
    public class When_using_BDD_style_language_for_instance_type_assertions
    {
        [Test]
        public void Should_allow_substitution_for_IsAssignableFrom()
        {
            5.ShouldBeAssignableFrom(typeof(int));
        }

        [Test]
        public void Should_allow_substitution_for_IsInstanceOfType()
        {
            5.ShouldBeInstanceOfType(typeof(int));
        }

        [Test]
        public void Should_allow_substitution_for_IsNotAssignableFrom()
        {
            5.ShouldNotBeAssignableFrom(typeof(string));
        }

        [Test]
        public void Should_allow_substitution_for_IsNotInstanceOfType()
        {
            5.ShouldNotBeInstanceOfType(typeof(double));
        }

        [Test]
        public void Should_allow_substitution_for_IsNotNull()
        {
            object value = "blarg";

            value.ShouldNotBeNull();
        }

        [Test]
        public void Should_allow_substitution_for_IsNull()
        {
            object value = null;

            value.ShouldBeNull();
        }
    }

    [TestFixture]
    public class When_using_BDD_style_language_for_instance_type_assertions_using_generics
    {
        [Test]
        public void Should_allow_substitution_for_IsAssignableFrom()
        {
            5.ShouldBeAssignableFrom<int>();
        }

        [Test]
        public void Should_allow_substitution_for_IsInstanceOfType()
        {
            5.ShouldBeInstanceOfType<int>();
        }

        [Test]
        public void Should_allow_substitution_for_IsNotAssignableFrom()
        {
            5.ShouldNotBeAssignableFrom<string>();
        }

        [Test]
        public void Should_allow_substitution_for_IsNotInstanceOfType()
        {
            5.ShouldNotBeInstanceOfType<double>();
        }
    }

    [TestFixture]
    public class When_specifying_exceptions_to_be_thrown
    {
        [Test]
        [ExpectedException(typeof(ThrowsException))]
        public void Should_fail_when_exception_is_of_a_different_type()
        {
            (typeof(SystemException)).ShouldBeThrownBy(() => { throw new ApplicationException(); });
        }

        [Test]
        public void Should_pass_when_exception_is_of_the_correct_type()
        {
            (typeof(ApplicationException)).ShouldBeThrownBy(() => { throw new ApplicationException(); });
        }

        [Test]
        [ExpectedException(typeof(ThrowsException))]
        public void Should_fail_when_exception_is_not_thrown()
        {
            (typeof(ApplicationException)).ShouldBeThrownBy(() => { });
        }

        [Test]
        [ExpectedException(typeof(ThrowsException))]
        public void Should_fail_when_exception_is_of_a_different_type_using_actions()
        {
            (typeof(SystemException)).ShouldBeThrownBy(() => { throw new ApplicationException(); });
        }

        [Test]
        public void Should_pass_when_exception_is_of_the_correct_type_using_actions()
        {
            (typeof(ApplicationException)).ShouldBeThrownBy(() => { throw new ApplicationException(); });
        }

        [Test]
        public void Should_return_exception_thrown_from_action()
        {
            var exception = new Action(() => { throw new ArgumentException(); }).GetException();

            exception.ShouldBeInstanceOfType<ArgumentException>();
        }

        [Test]
        public void Should_pass_when_exception_is_correct_type()
        {
            Action action = () => { throw new ArgumentException("blerg"); };
            action.ShouldThrow<ArgumentException>();
        }

        [Test, ExpectedException(typeof(FalseException), ExpectedMessage = "Exception of type <System.ArgumentException> expected but no exception occurred")]
        public void Should_fail_when_no_exception_occurs()
        {
            Action action = () => { };
            action.ShouldThrow<ArgumentException>();
        }

        [Test, ExpectedException(typeof(IsTypeException))]
        public void Should_fail_when_exception_is_not_correct_type()
        {
            Action action = () => { throw new ApplicationException("blerg"); };
            action.ShouldThrow<ArgumentException>();
        }

        [Test]
        public void Should_pass_when_exception_is_correct_type_and_message_is_correct_type()
        {
            Action action = () => { throw new ArgumentException("blerg"); };
            action.ShouldThrow<ArgumentException>().WithExceptionMessage("blerg");
        }
    }

    [TestFixture]
    public class When_using_BDD_style_language_for_double_assertions
    {
        [Test]
        public void Should_allow_substitution_for_Greater()
        {
            5.1.ShouldBeGreaterThan(4.5);
        }

        [Test]
        public void Should_allow_substitution_for_GreaterOrEqual()
        {
            5.1.ShouldBeGreaterThanOrEqualTo(5.1);
        }

        [Test]
        public void Should_allow_substitution_for_IsNaN()
        {
            double.NaN.ShouldBeNaN();
        }

        [Test]
        public void Should_allow_substitution_for_Less()
        {
            5.1.ShouldBeLessThan(5.2);
        }

        [Test]
        public void Should_allow_substitution_for_LessOrEqualTo()
        {
            5.1.ShouldBeLessThanOrEqualTo(6.2);
        }

        [Test]
        public void Should_allow_substitiution_for_AreApproximatelyEqual()
        {
            5.1.ShouldApproximatelyEqual(5.2, 0.11);
        }

        //[Test]
        //public void Should_allow_substitiution_for_AreNotApproximatelyEqual()
        //{
        //    5.1.ShouldNotApproximatelyEqual(5.3, 0.1);
        //}
    }
}
