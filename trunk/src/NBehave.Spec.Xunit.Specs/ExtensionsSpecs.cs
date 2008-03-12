using System;
using Xunit;
using Context = NUnit.Framework.TestFixtureAttribute;
using Specification = NUnit.Framework.TestAttribute;
using ExpectedExceptionNUnit = NUnit.Framework.ExpectedExceptionAttribute;

namespace NBehave.Spec.Xunit.Specs
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
        public void Should_allow_substitution_for_Equal()
        {
            int i, j;
            i = 5;
            j = 5;

            i.ShouldEqual(j);
        }

        [Specification]
        public void Should_allow_substitution_for_NotEqual()
        {
            int i = 5;
            int j = 6;

            i.ShouldNotEqual(j);
        }

        [Specification]
        public void Should_allow_substitution_for_NotSame()
        {
            object test1 = "blarg";
            object test2 = "splorg";

            test2.ShouldNotBeTheSameAs(test1);
        }

        [Specification]
        public void Should_allow_substitution_for_Same()
        {
            object test1 = "blarg";
            object test2 = test1;

            test2.ShouldBeTheSameAs(test1);
        }
    }

    [Context]
    public class When_using_BDD_style_language_for_instance_type_assertions_using_generics
    {
        [Specification]
        public void Should_allow_substitution_for_IsType()
        {
            "blarg".ShouldBeInstanceOf(typeof(string));
        }

        [Specification]
        public void Should_allow_generic_substitution_for_IsType()
        {
            "blarg".ShouldBeInstanceOf<string>();
        }

        [Specification]
        public void Should_allow_substitution_for_IsNotType()
        {
            "blarg".ShouldNotBeInstanceOf(typeof(int));
        }

        [Specification]
        public void Should_allow_generic_substitution_for_IsNotType()
        {
            "blarg".ShouldNotBeInstanceOf<int>();
        }
    }

    [Context]
    public class When_using_BDD_style_language_for_null_assertions
    {
        [Specification]
        public void Should_allow_substitution_for_Null()
        {
            ((string)null).ShouldBeNull();
        }

        [Specification]
        public void Should_allow_substitution_for_NotNull()
        {
            "blarg".ShouldNotBeNull();
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
        public void Should_allow_substitutions_for_NotContains()
        {
            "blarg".ShouldNotContain("asdf");
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
        public void Should_allow_substitution_for_Empty()
        {
            int[] values = {};
            values.ShouldBeEmpty();
        }

        [Specification]
        public void Should_allow_substitution_for_NotEmpty()
        {
            int[] values = {1};
            values.ShouldNotBeEmpty();
        }

    }

    [Context]
    public class When_using_BDD_style_language_for_range_assertions
    {
        [Specification]
        public void Should_allow_substitutions_for_InRange()
        {
            int i, j, k;
            i = 5;
            j = 0;
            k = 10;

            i.ShouldBeBetween(j, k);
        }

        [Specification]
        public void Should_allow_substitutions_for_NotInRange()
        {
            int i, j, k;
            i = 5;
            j = 6;
            k = 10;

            i.ShouldNotBeBetween(j, k);
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
        [ExpectedExceptionNUnit(typeof(ThrowsException))]
        public void Should_fail_when_exception_is_of_a_different_type()
        {
            ExceptionThrower thrower = new ExceptionThrower();

            thrower.ShouldThrow(typeof(ApplicationException)).WhenCalling(x => x.ThrowException(5));
        }

        [Specification]
        [ExpectedExceptionNUnit(typeof(ThrowsException))]
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
}