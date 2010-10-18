using System;
using NUnit.Framework;
using NBehave.Narrator.Framework;

namespace NBehave.Narrator.Framework.Specifications.Text
{
	public class SomeSpec {}
	public class Some_Spec {}
	public class Some {}
	public class Spec {}

	[TestFixture]
	public class IgnoreSpaceAndUnderScoreMatcherSpec
	{		
		[TestCase("Spec.story", typeof(Spec))]
		[TestCase("Some Spec.story", typeof(SomeSpec))]
		[TestCase("Some Spec.scenario", typeof(SomeSpec))]
		[TestCase("Some Spec.story", typeof(Some_Spec))]
		[TestCase("Some_Spec.story", typeof(SomeSpec))]
		[TestCase("Some_Spec.story", typeof(Some_Spec))]
		public void Should_be_considired_equal(string fileName, Type typeToMatch)
		{
			IFileMatcher matcher = new IgnoreSpaceAndUnderScoreMatcher(typeToMatch);
			Assert.IsTrue(matcher.IsMatch(fileName));
		}

		[TestCase("Spec.story", typeof(SomeSpec))]
		[TestCase("SpecSome.story", typeof(SomeSpec))]
		[TestCase("Some-Spec.scenario", typeof(SomeSpec))]
		[TestCase("Some.Spec.scenario", typeof(SomeSpec))]
		[TestCase("Some Spec.story", typeof(Spec))]
		[TestCase("Some_Spec.story", typeof(Some))]
		public void Should_not_be_considired_equal(string fileName, Type typeToMatch)
		{
			IFileMatcher matcher = new IgnoreSpaceAndUnderScoreMatcher(typeToMatch);
			Assert.IsFalse(matcher.IsMatch(fileName));
		}
	}
}
