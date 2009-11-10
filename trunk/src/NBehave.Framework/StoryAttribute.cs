using System;

namespace NBehave.Narrator.Framework
{
	[AttributeUsage(AttributeTargets.Method)]
	[Obsolete("You should switch to text scenarios, read more here http://nbehave.codeplex.com/wikipage?title=With%20textfiles%20and%20ActionSteps&referringTitle=Examples")]
	public class StoryAttribute : Attribute
	{
	}
}