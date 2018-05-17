using System;
using System.IO;
using System.Text.RegularExpressions;
using Gallio.Framework.Assertions;
using NUnit.Framework;

namespace NBehave.Spec.MbUnit.Specs
{
    public abstract class ConsoleRedirect
    {
        private TextWriter outStream;

        [SetUp]
        public void CaptureConsoleOut()
        {
            outStream = Console.Out;
            Console.SetOut(new StringWriter());
        }

        [TearDown]
        public void ResetConsoleOut()
        {
            Console.SetOut(outStream);
        }
    }

	[TestFixture]
    public class When_using_BDD_style_language_for_boolean_assertions : ConsoleRedirect
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
    public class When_using_BDD_style_language_for_equality_assertions : ConsoleRedirect
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
    public class When_using_BDD_style_language_for_collection_assertions : ConsoleRedirect
	{
		[Test]
		public void Should_allow_substitution_for_Contains_on_collections()
		{
			int[] vals = { 5, 6, 7, 8 };

			vals.ShouldContain(6);
		}

		[Test]
		public void Should_allow_substitution_for_DoesNotContains_on_collections()
		{
			int[] vals = { 5, 6, 7, 8 };

			vals.ShouldNotContain(9);
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
    public class When_using_BDD_style_language_for_integer_assertions : ConsoleRedirect
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
    public class When_using_BDD_style_language_for_string_assertions : ConsoleRedirect
	{
		[Test]
		public void Should_allow_substitution_for_IsNotEmpty_for_strings()
		{
			"blarg".ShouldNotBeEmpty();
		}

		[Test]
		public void Should_allow_substitution_for_IsEmpty_for_strings()
		{
			string.Empty.ShouldBeEmpty();
		}

		[Test]
		public void Should_allow_substitution_for_EndsWith()
		{
			"Lorem ipsum dolor sit amet.".ShouldEndWith("amet.");
		}

		[Test]
		public void Should_allow_substitution_for_StartsWith()
		{
			"Lorem ipsum dolor sit amet.".ShouldStartWith("Lorem");
		}

		[Test]
		public void Should_allow_substitution_for_FullMatch()
		{
			"I have 5 euros in my pocket".ShouldFullyMatch(@"I have \d+ euros in my pocket");
		}

		[Test]
		public void Should_allow_substitution_for_Like()
		{
			"I have 5 euros in my pocket".ShouldMatch(@"\d+ euros");
		}

		[Test]
		public void Should_allow_substitutions_for_DoesNotMatch()
		{
			"blarg".ShouldNotMatch(new Regex("asdf"));
		}
	
		[Test]
		public void Should_allow_substitution_for_ShouldNotContain_for_string()
		{
			"Lorem ipsum dolor sit amet.".ShouldNotContain("foo");
		}

		[Test]
		public void Should_allow_substitution_for_ShouldNotContain__for_string_failing()
		{
            Assert.Throws<AssertionFailureException>(
		        () => "Lorem ipsum dolor sit amet.".ShouldNotContain("ipsum")
            );
		}

		[Test]
		public void Should_allow_substitution_for_ShouldContain_for_string()
		{
			var str = "Hello";
			str.ShouldContain("Hell");
		}

		[Test]
		public void Should_allow_substitution_for_ShouldContain_for_string_failing()
		{
			var str = "Hello";
            Assert.Throws<AssertionFailureException>(
			    () => str.ShouldContain("Foo")
            );
		}
}

	[TestFixture]
    public class When_using_BDD_style_language_for_instance_type_assertions : ConsoleRedirect
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
    public class When_using_BDD_style_language_for_instance_type_assertions_using_generics : ConsoleRedirect
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
    public class When_specifying_exceptions_to_be_thrown : ConsoleRedirect
	{
		[Test]
		public void Should_fail_when_exception_is_a_different_type()
		{
			Assert.Throws<AssertionFailureException>(() =>
			{
			(typeof(SystemException)).ShouldBeThrownBy(
				delegate { throw new ApplicationException(); });
			});

		}

		[Test]
		public void Should_fail_when_exception_is_not_thrown()
		{
			Assert.Throws<AssertionFailureException>(() => { (typeof(ApplicationException)).ShouldBeThrownBy(delegate { }); });
		}

		[Test]
		public void Should_pass_when_exception_is_thrown()
		{
			(typeof(ApplicationException)).ShouldBeThrownBy(
				delegate { throw new ApplicationException(); });

		}

		[Test]
		public void Should_return_exception_thrown_from_action()
		{
			var exception = new Action(() => { throw new ArgumentException(); }).GetException();

			exception.ShouldBeInstanceOfType<ArgumentException>();
		}
	}

	[TestFixture]
    public class When_using_BDD_style_language_for_double_assertions : ConsoleRedirect
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

		[Test]
		public void Should_allow_substitiution_for_AreNotApproximatelyEqual()
		{
			5.1.ShouldNotApproximatelyEqual(5.3, 0.1);
		}
	}
}
