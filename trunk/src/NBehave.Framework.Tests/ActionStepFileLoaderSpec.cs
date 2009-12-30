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
                                "GreetingSystem.txt",
                                "GreetingSystemWithScenarioTitle.txt"
                            };
            var stories = _actionStepFileLoader.Load(files);
            Assert.That(stories.Count, Is.EqualTo(2));
        }
    }
}