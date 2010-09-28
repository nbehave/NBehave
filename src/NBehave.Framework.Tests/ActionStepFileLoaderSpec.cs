using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Context = NUnit.Framework.TestFixtureAttribute;
using Specification = NUnit.Framework.TestAttribute;

namespace NBehave.Narrator.Framework.Specifications
{
	[Context]
	public class ActionStepFileLoaderSpec
	{
		private ActionStepFileLoader _actionStepFileLoader;

		[SetUp]
		public void Establish_context()
		{
			_actionStepFileLoader = new ActionStepFileLoader(new StringStepRunner(new ActionCatalog()));
		}

		[Specification]
		public void Should_treat_each_file_as_a_story()
		{
			var files = new[]
			{
				"Features\\GreetingSystem.feature",
				"Features\\GreetingSystemWithScenarioTitle.feature"
			};
			var stories = _actionStepFileLoader.Load(files);
			Assert.That(stories.Count, Is.EqualTo(2));
		}

		[Specification]
		public void Should_have_Source_set_on_step()
		{
			var files = new[]
			{
				"Features\\GreetingSystem.feature",
			};
			var stories = _actionStepFileLoader.Load(files);

			Assert.That(stories[0].Scenarios.First().Steps.First().Source, Is.Not.Null);
			Assert.That(stories[0].Scenarios.First().Steps.First().Source, Is.Not.EqualTo(string.Empty));
		}

		[Specification]
		public void Should_be_able_to_use_relative_paths_with_dots()
		{
			IEnumerable<string> locations = new[] { @"..\*.*" };
			var steps = _actionStepFileLoader.Load(locations);
			Assert.That(steps, Is.Not.Null);
		}
	}
}