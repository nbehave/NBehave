using System;
using System.Collections;
using System.Text.RegularExpressions;
using NUnit.Framework;
using Context = NUnit.Framework.TestFixtureAttribute;

namespace NBehave.Spec.NUnit.Specs
{
    [Context]
    public class When_using_BDD_style_language_for_boolean_assertions
    {
        [Specification]
        public void Should_allow_substitution_for_IsFalse()
        {
            false.ShouldBeFalse();
        }

        [Specification]
        public void Should_allow_substitution_for_IsTrue()
        {
            true.ShouldBeTrue();
        }
    }

    [Context]
    public class When_using_BDD_style_language_for_equality_assertions
    {
        [Specification]
        public void Should_allow_substitution_for_AreEqual()
        {
            int i, j;
            i = 5;
            j = 5;

            i.ShouldEqual(j);
        }

        [Specification]
        public void Should_allow_substitution_for_AreNotEqual()
        {
            int i = 5;
            int j = 6;

            i.ShouldNotEqual(j);
        }

        [Specification]
        public void Should_allow_substitution_for_AreNotSame()
        {
            object test1 = "blarg";
            object test2 = "splorg";

            test2.ShouldNotBeTheSameAs(test1);
        }

        [Specification]
        public void Should_allow_substitution_for_AreSame()
        {
            object test1 = "blarg";
            object test2 = test1;

            test2.ShouldBeTheSameAs(test1);
        }
    }

    [Context]
    public class When_using_BDD_style_language_for_collection_assertions
    {
        [Specification]
        public void Should_allow_substitution_for_Contains_on_collections()
        {
            int[] vals = { 5, 6, 7, 8 };

            vals.ShouldContain(6);
        }

        [Specification]
        public void Should_allow_substitution_for_ShouldNotContain_on_collections()
        {
            int[] vals = { 5, 6, 7, 8 };

            vals.ShouldNotContain(1);
        }

        [Specification]
        public void Should_allow_substitution_for_IsEmpty_for_collections()
        {
            int[] vals = { };

            vals.ShouldBeEmpty();
        }

        [Specification]
        public void Should_allow_substitution_for_IsNotEmpty_for_collections()
        {
            int[] vals = { 1, 2, 3 };

            vals.ShouldNotBeEmpty();
        }

        [Specification]
        public void should_allow_substitution_for_contains_on_ienumerable()
        {
            IEnumerable lst = new ArrayList { 1, 2, 3, 4 };
            lst.ShouldContain(3);
        }

        [Specification]
        public void Should_allow_substitutions_for_AreEqual()
        {
            int[] values1 = { 4, 5, 6 };
            int[] values2 = { 4, 5, 6 };

            values1.ShouldBeEqualTo(values2);
        }

        [Specification]
        public void Should_allow_substitutions_for_AreNotEqual()
        {
            int[] values1 = { 4, 5, 6 };
            int[] values2 = { 5, 6, 4 };

            values1.ShouldNotBeEqualTo(values2);
        }

        [Specification]
        public void Should_allow_substitutions_for_AreEquivalent()
        {
            int[] values1 = { 4, 5, 6 };
            int[] values2 = { 6, 4, 5 };

            values1.ShouldBeEquivalentTo(values2);
        }

        [Specification]
        public void Should_allow_substitutions_for_AreNotEquivalent()
        {
            int[] values1 = { 4, 5, 6 };
            int[] values2 = { 6, 4, 7 };

            values1.ShouldNotBeEquivalentTo(values2);
        }
    }

    [Context]
    public class When_using_BDD_style_language_for_integer_assertions
    {
        [Specification]
        public void Should_allow_substitution_for_Greater()
        {
            5.ShouldBeGreaterThan(4);
        }

        [Specification]
        public void Should_allow_substitution_for_GreaterOrEqual()
        {
            5.ShouldBeGreaterThanOrEqualTo(5);
        }

        [Specification]
        public void Should_allow_substitution_for_IsNaN()
        {
            double.NaN.ShouldBeNaN();
        }

        [Specification]
        public void Should_allow_substitution_for_Less()
        {
            5.ShouldBeLessThan(6);
        }

        [Specification]
        public void Should_allow_substitution_for_LessOrEqualTo()
        {
            5.ShouldBeLessThanOrEqualTo(6);
        }
    }

    [Context]
    public class When_using_BDD_style_language_for_string_assertions
    {

        [Specification]
        public void Should_allow_substitutions_for_EndsWith()
        {
            "blarg".ShouldEndWith("arg");
        }

        [Specification]
        public void Should_allow_substitutions_for_Matches()
        {
            "blarg".ShouldMatch(new Regex("blarg"));
        }

        [Specification]
        public void Should_allow_substitution_for_IsEmpty_for_strings()
        {
            string.Empty.ShouldBeEmpty();
        }

        [Specification]
        public void Should_allow_substitution_for_IsNotEmpty_for_strings()
        {
            "blarg".ShouldNotBeEmpty();
        }

        [Specification]
        public void Should_allow_substitutions_for_DoesNotMatch()
        {
            "blarg".ShouldNotMatch(new Regex("asdf"));
        }

        [Specification]
        public void Should_allow_substitutions_for_StartsWith()
        {
            "blarg".ShouldStartWith("bl");
        }
    }

    [Context]
    public class When_using_BDD_style_language_for_instance_type_assertions
    {
        [Specification]
        public void Should_allow_substitution_for_IsAssignableFrom()
        {
            5.ShouldBeAssignableFrom(typeof(int));
        }

        [Specification]
        public void Should_allow_substitution_for_IsInstanceOfType()
        {
            5.ShouldBeInstanceOfType(typeof(int));
        }

        [Specification]
        public void Should_allow_substitution_for_IsNotAssignableFrom()
        {
            5.ShouldNotBeAssignableFrom(typeof(string));
        }

        [Specification]
        public void Should_allow_substitution_for_IsNotInstanceOfType()
        {
            5.ShouldNotBeInstanceOfType(typeof(double));
        }

        [Specification]
        public void Should_allow_substitution_for_IsNotNull()
        {
            object value = "blarg";

            value.ShouldNotBeNull();
        }

        [Specification]
        public void Should_allow_substitution_for_IsNull()
        {
            object value = null;

            value.ShouldBeNull();
        }
    }

    [Context]
    public class When_using_BDD_style_language_for_instance_type_assertions_using_generics
    {
        [Specification]
        public void Should_allow_substitution_for_IsAssignableFrom()
        {
            5.ShouldBeAssignableFrom<int>();
        }

        [Specification]
        public void Should_allow_substitution_for_IsInstanceOfType()
        {
            5.ShouldBeInstanceOf<int>();
        }

        [Specification]
        public void Should_allow_substitution_for_IsNotAssignableFrom()
        {
            5.ShouldNotBeAssignableFrom<string>();
        }

        [Specification]
        public void Should_allow_substitution_for_IsNotInstanceOfType()
        {
            5.ShouldNotBeInstanceOf<double>();
        }
    }

    [Context]
    public class When_specifying_exceptions_to_be_thrown
    {
        [Specification]
        [ExpectedException(typeof(AssertionException))]
        public void Should_fail_when_exception_is_of_a_different_type()
        {
            (typeof(SystemException)).ShouldBeThrownBy(() => { throw new ApplicationException(); });
        }

        [Specification]
        public void Should_pass_when_exception_is_of_the_correct_type()
        {
            (typeof(ApplicationException)).ShouldBeThrownBy(() => { throw new ApplicationException(); });
        }

        [Specification]
        [ExpectedException(typeof(AssertionException))]
        public void Should_fail_when_exception_is_not_thrown()
        {
            (typeof(ApplicationException)).ShouldBeThrownBy(() => { });
        }

        [Specification]
        [ExpectedException(typeof(AssertionException))]
        public void Should_fail_when_exception_is_of_a_different_type_using_actions()
        {
            (typeof(SystemException)).ShouldBeThrownBy(() => { throw new ApplicationException(); });
        }

        [Specification]
        public void Should_pass_when_exception_is_of_the_correct_type_using_actions()
        {
            (typeof(ApplicationException)).ShouldBeThrownBy(() => { throw new ApplicationException(); });
        }

        [Specification]
        public void Should_return_exception_thrown_from_action()
        {
            Exception exception = new Action(() => { throw new ArgumentException(); }).GetException();

            exception.ShouldBeInstanceOf<ArgumentException>();
        }
    }
}
