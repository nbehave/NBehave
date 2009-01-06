using System;
using System.Collections;
using NBehave.Spec.NUnit.Underscore;
using NUnit.Framework;
using Context = NUnit.Framework.TestFixtureAttribute;
using Specification = NUnit.Framework.TestAttribute;

namespace NBehave.Spec.NUnit.Specs
{
	[Context]
	public class When_using_BDD_style_language_for_boolean_assertions
	{
		[Specification]
		public void Should_allow_substitution_for_IsFalse()
		{
			false.ShouldBeFalse();

			false.should_be_false();
		}

		[Specification]
		public void Should_allow_substitution_for_IsTrue()
		{
			true.ShouldBeTrue();

			true.should_be_true();
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
			i.should_equal(j);
		}

		[Specification]
		public void Should_allow_substitution_for_AreNotEqual()
		{
			int i = 5;
			int j = 6;

			i.ShouldNotEqual(j);
			i.should_not_equal(j);
		}

		[Specification]
		public void Should_allow_substitution_for_AreNotSame()
		{
			object test1 = "blarg";
			object test2 = "splorg";

			test2.ShouldNotBeTheSameAs(test1);

			test2.should_not_be_the_same_as(test1);
		}

		[Specification]
		public void Should_allow_substitution_for_AreSame()
		{
			object test1 = "blarg";
			object test2 = test1;

			test2.ShouldBeTheSameAs(test1);

			test2.should_be_the_same_as(test1);
		}
	}

	[Context]
	public class When_using_BDD_style_language_for_collection_assertions
	{
		[Specification]
		public void Should_allow_substitution_for_Contains_on_collections()
		{
			int[] vals = {5, 6, 7, 8};

			vals.ShouldContain(6);

			vals.should_contain(6);
		}

		[Specification]
		public void Should_allow_substitution_for_IsEmpty_for_collections()
		{
			int[] vals = {};

			vals.ShouldBeEmpty();

			vals.should_be_empty();
		}

		[Specification]
		public void Should_allow_substitution_for_IsNotEmpty_for_collections()
		{
			int[] vals = {1, 2, 3};

			vals.ShouldNotBeEmpty();

			vals.should_not_be_empty();
		}
	
        [Specification]
        public void should_allow_substitution_for_contains_on_ienumerable()
        {
            IEnumerable lst = new ArrayList {1, 2, 3, 4};
            lst.ShouldContain(3);
            lst.should_contain(3);
        }
    }

	[Context]
	public class When_using_BDD_style_language_for_integer_assertions
	{
		[Specification]
		public void Should_allow_substitution_for_Greater()
		{
			5.ShouldBeGreaterThan(4);

			5.should_be_greater_than(4);
		}

		[Specification]
		public void Should_allow_substitution_for_GreaterOrEqual()
		{
			5.ShouldBeGreaterThanOrEqualTo(5);

			5.should_be_greater_than_or_equal_to(5);
		}

		[Specification]
		public void Should_allow_substitution_for_IsNaN()
		{
			double.NaN.ShouldBeNaN();

			double.NaN.should_be_NaN();
		}

		[Specification]
		public void Should_allow_substitution_for_Less()
		{
			5.ShouldBeLessThan(6);

			5.should_be_less_than(6);
		}

		[Specification]
		public void Should_allow_substitution_for_LessOrEqualTo()
		{
			5.ShouldBeLessThanOrEqualTo(6);

			5.should_be_less_than_or_equal_to(6);
		}
	}

	[Context]
	public class When_using_BDD_style_language_for_string_assertions
	{
		[Specification]
		public void Should_allow_substitution_for_IsEmpty_for_strings()
		{
			string.Empty.ShouldBeEmpty();

			string.Empty.should_be_empty();
		}

		[Specification]
		public void Should_allow_substitution_for_IsNotEmpty_for_strings()
		{
			"blarg".ShouldNotBeEmpty();

			"lost".should_not_be_empty();
		}
	}

	[Context]
	public class When_using_BDD_style_language_for_instance_type_assertions
	{
		[Specification]
		public void Should_allow_substitution_for_IsAssignableFrom()
		{
			5.ShouldBeAssignableFrom(typeof (int));

			5.should_be_assignable_from(typeof (int));
		}

		[Specification]
		public void Should_allow_substitution_for_IsInstanceOfType()
		{
			5.ShouldBeInstanceOfType(typeof (int));

			5.should_be_instance_of_type(typeof (int));
		}

		[Specification]
		public void Should_allow_substitution_for_IsNotAssignableFrom()
		{
			5.ShouldNotBeAssignableFrom(typeof (string));
		}

		[Specification]
		public void Should_allow_substitution_for_IsNotInstanceOfType()
		{
			5.ShouldNotBeInstanceOfType(typeof (double));

			5.should_not_be_instance_of_type(typeof (double));
		}

		[Specification]
		public void Should_allow_substitution_for_IsNotNull()
		{
			object value = "blarg";

			value.ShouldNotBeNull();
			5.should_not_be_assignable_from(typeof (string));
		}

		[Specification]
		public void Should_allow_substitution_for_IsNull()
		{
			object value = null;

			value.ShouldBeNull();

			value.should_be_null();
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

			5.should_be_instance_of<int>();
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
		[ExpectedException(typeof (AssertionException))]
		public void Should_fail_when_exception_is_a_different_type()
		{
			(typeof (SystemException)).ShouldBeThrownBy(
				() =>
			{
				throw new ApplicationException();
			}
		)
			;
		}

		[Specification]
		[ExpectedException(typeof (AssertionException))]
		public void Should_fail_when_exception_is_a_different_type_underscores()
		{
			(typeof (SystemException)).should_be_thrown_by(
				() =>
			{
				throw new ApplicationException();
			}
		)
			;
		}


		[Specification]
		[ExpectedException(typeof (AssertionException))]
		public void Should_fail_when_exception_is_not_thrown()
		{
			(typeof (ApplicationException)).ShouldBeThrownBy(() =>
			{
				;
			}
		)
			;

			(typeof (ApplicationException)).should_be_thrown_by(() =>
			{
				;
			}
		)
			;
		}

		[Specification]
		public void Should_pass_when_exception_is_thrown()
		{
			(typeof (ApplicationException)).ShouldBeThrownBy(
				() =>
			{
				throw new ApplicationException();
			}
		)
			;
		}

		[Specification]
		public void Should_pass_when_exception_is_thrown_Underscores()
		{
			(typeof (ApplicationException)).should_be_thrown_by(
				() =>
			{
				throw new ApplicationException();
			}
		)
			;
		}
	}
}