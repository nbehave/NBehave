using System;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;

namespace NBehave.Spec.MSTest10.Specifications
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
            int i, j;
            i = 5;
            j = 6;

            i.ShouldNotEqual(j);
        }

        [Test]
        public void Should_allow_substitution_for_AreSame()
        {
            object test1 = "blarg";
            object test2 = test1;

            test1.ShouldBeTheSameAs(test2);
        }

        [Test]
        public void Should_allow_substitution_for_AreNotSame()
        {
            object test1 = "blarg";
            object test2 = "splorg";

            test1.ShouldNotBeTheSameAs(test2);
        }
    }

    [TestFixture]
    public class When_using_BDD_style_language_for_instance_type_assertions_using_generics
    {
        [Test]
        public void Should_allow_substitution_for_IsAssignableFrom()
        {
            5.ShouldBeAssignableFrom(typeof(int));
        }
        
        [Test]
        public void Should_allow_substitution_for_IsNotAssignableFrom()
        {
            5.ShouldNotBeAssignableFrom(typeof(string));
        }

        [Test]
        public void Should_allow_substitution_for_IsInstanceOfType()
        {
            "blarg".ShouldBeInstanceOfType(typeof(string));
        }

        [Test]
        public void Should_allow_generic_substitution_for_IsInstanceOfType()
        {
            "blarg".ShouldBeInstanceOfType<string>();
        }

        [Test]
        public void Should_allow_substitution_for_IsNotInstanceOfType()
        {
            "blarg".ShouldNotBeInstanceOfType(typeof(int));
        }

        [Test]
        public void Should_allow_generic_substitution_for_IsNotInstanceOfType()
        {
            "blarg".ShouldNotBeInstanceOfType<int>();
        }
    }

    [TestFixture]
    public class When_using_BDD_style_language_for_null_assertions
    {
        [Test]
        public void Should_allow_substitution_for_IsNull()
        {
            ((string)null).ShouldBeNull();
        }

        [Test]
        public void Should_allow_substitution_for_IsNotNull()
        {
            "blarg".ShouldNotBeNull();
        }
    }

    [TestFixture]
    public class When_specifying_an_exception_to_be_thrown
    {
        [Test]
        [NUnit.Framework.ExpectedException(typeof(AssertFailedException))]
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
        [NUnit.Framework.ExpectedException(typeof(AssertFailedException))]
        public void Should_fail_when_exception_is_not_thrown()
        {
            (typeof(ApplicationException)).ShouldBeThrownBy(() => { });
        }

        [Test]
        [NUnit.Framework.ExpectedException(typeof(AssertFailedException))]
        public void Should_fail_when_exception_is_of_a_different_type_using_actions()
        {
            (typeof(ApplicationException)).ShouldBeThrownBy(() => { throw new ArgumentException(); });
        }

        [Test]
        public void Should_pass_when_exception_is_of_the_correct_type_using_actions()
        {
            (typeof(ApplicationException)).ShouldBeThrownBy(() => { throw new ApplicationException(); });
        }

        [Test]
        public void Should_return_exception_thrown_from_action()
        {
            Exception exception = new Action(() => { throw new ArgumentException(); }).GetException();

            exception.ShouldBeInstanceOfType<ArgumentException>();
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
        public void Should_allow_substitutions_for_StartsWith()
        {
            "blarg".ShouldStartWith("bl");
        }

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
        public void Should_allow_substitutions_for_DoesNotMatch()
        {
            "blarg".ShouldNotMatch(new Regex("asdf"));
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
        public void Should_allow_substitution_for_ShouldNotContain_for_string()
        {
            "Lorem ipsum dolor sit amet.".ShouldNotContain("foo");
        }

        [Test, NUnit.Framework.ExpectedException(typeof(AssertFailedException))]
        public void Should_allow_substitution_for_ShouldNotContain__for_string_failing()
        {
            "Lorem ipsum dolor sit amet.".ShouldNotContain("ipsum");
        }

        [Test]
        public void Should_allow_substitution_for_ShouldContain_for_string()
        {
            string str = "Hello";
            str.ShouldContain("Hell");
        }

        [Test, NUnit.Framework.ExpectedException(typeof(AssertFailedException))]
        public void Should_allow_substitution_for_ShouldContain_for_string_failing()
        {
            string str = "Hello";
            str.ShouldContain("Foo");
        }
    }

    [TestFixture]
    public class When_using_BDD_style_language_for_collection_assertions
    {
        [Test]
        public void Should_allow_substitutions_for_Contains()
        {
            int[] values = { 4, 5, 6 };
            values.ShouldContain(4);
        }

        [Test]
        public void Should_allow_substitutions_for_DoesNotContain()
        {
            int[] values = { 4, 5, 6 };
            values.ShouldNotContain(7);
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
    }
}