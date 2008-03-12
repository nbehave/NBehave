using System;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ExpectedExceptionNUnit = NUnit.Framework.ExpectedExceptionAttribute;
using Context = NUnit.Framework.TestFixtureAttribute;
using Specification = NUnit.Framework.TestAttribute;
using NBehave.Spec.MSTest;

namespace NBehave.Spec.MSTest.Specs
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
            int i, j;
            i = 5;
            j = 6;

            i.ShouldNotEqual(j);
        }

        [Specification]
        public void Should_allow_substitution_for_AreSame()
        {
            object test1 = "blarg";
            object test2 = test1;

            test1.ShouldBeTheSameAs(test2);
        }

        [Specification]
        public void Should_allow_substitution_for_AreNotSame()
        {
            object test1 = "blarg";
            object test2 = "splorg";

            test1.ShouldNotBeTheSameAs(test2);
        }
    }

    [Context]
    public class When_using_BDD_style_language_for_instance_type_assertions_using_generics
    {
        [Specification]
        public void Should_allow_substitution_for_IsInstanceOfType()
        {
            "blarg".ShouldBeInstanceOf(typeof(string));
        }

        [Specification]
        public void Should_allow_generic_substitution_for_IsInstanceOfType()
        {
            "blarg".ShouldBeInstanceOf<string>();
        }

        [Specification]
        public void Should_allow_substitution_for_IsNotInstanceOfType()
        {
            "blarg".ShouldNotBeInstanceOf(typeof(int));
        }

        [Specification]
        public void Should_allow_generic_substitution_for_IsNotInstanceOfType()
        {
            "blarg".ShouldNotBeInstanceOf<int>();
        }
    }

    [Context]
    public class When_using_BDD_style_language_for_null_assertions
    {
        [Specification]
        public void Should_allow_substitution_for_IsNull()
        {
            ((string)null).ShouldBeNull();
        }

        [Specification]
        public void Should_allow_substitution_for_IsNotNull()
        {
            "blarg".ShouldNotBeNull();
        }
    }

    [Context]
    public class When_specifying_an_exception_to_be_thrown
    {
        public class ExceptionThrower
        {
            public void ThrowException(int value)
            {
                throw new ArgumentException();
            }
        }

        [Specification]
        [ExpectedExceptionNUnit(typeof(AssertFailedException))]
        public void Should_fail_when_exception_is_of_a_different_type()
        {
            ExceptionThrower thrower = new ExceptionThrower();

            thrower.ShouldThrow(typeof (ApplicationException)).WhenCalling(x => x.ThrowException(5));
        }

        [Specification]
        [ExpectedExceptionNUnit(typeof(AssertFailedException))]
        public void Should_fail_when_exception_is_of_a_different_type_using_actions()
        {
            (typeof(ApplicationException)).ShouldBeThrownBy(() => { throw new ArgumentException(); });
        }

        [Specification]
        public void Should_return_exception_thrown_from_action()
        {
            Exception exception = new Action(() => { throw new ArgumentException(); }).GetException();

            exception.ShouldBeInstanceOf<ArgumentException>();
        }
        
        [Specification]
        public void Should_pass_when_exception_is_of_the_correct_type()
        {
            ExceptionThrower thrower = new ExceptionThrower();

            thrower.ShouldThrow(typeof(ArgumentException)).WhenCalling(x => x.ThrowException(5));
        }

        [Specification]
        public void Should_pass_when_exception_is_of_the_correct_type_using_actions()
        {
            (typeof(ApplicationException)).ShouldBeThrownBy(() => { throw new ApplicationException(); });
        }

    }

    [Context]
    public class When_using_BDD_style_language_for_string_assertions
    {
        [Specification]
        public void Should_allow_substitutions_for_Contains()
        {
            "blarg".ShouldContain("lar");
        }

        [Specification]
        public void Should_allow_substitutions_for_StartsWith()
        {
            "blarg".ShouldStartWith("bl");
        }

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
        public void Should_allow_substitutions_for_DoesNotMatch()
        {
            "blarg".ShouldNotMatch(new Regex("asdf"));
        }
    }

    [Context]
    public class When_using_BDD_style_language_for_collection_assertions
    {
        [Specification]
        public void Should_allow_substitutions_for_Contains()
        {
            int[] values = { 4, 5, 6 };
            values.ShouldContain(4);
        }

        [Specification]
        public void Should_allow_substitutions_for_DoesNotContain()
        {
            int[] values = { 4, 5, 6 };
            values.ShouldNotContain(7);
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
    }

}