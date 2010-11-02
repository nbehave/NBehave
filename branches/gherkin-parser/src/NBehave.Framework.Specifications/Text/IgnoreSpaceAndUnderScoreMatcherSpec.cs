using System;
using NUnit.Framework;

namespace NBehave.Narrator.Framework.Specifications.Text
{
	public class SomeSpec {}
// ReSharper disable InconsistentNaming
	public class Some_Spec {}
// ReSharper restore InconsistentNaming
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
		public void ShouldBeConsidiredEqual(string fileName, Type typeToMatch)
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
		public void ShouldNotBeConsidiredEqual(string fileName, Type typeToMatch)
		{
			IFileMatcher matcher = new IgnoreSpaceAndUnderScoreMatcher(typeToMatch);
			Assert.IsFalse(matcher.IsMatch(fileName));
		}
	}
}
