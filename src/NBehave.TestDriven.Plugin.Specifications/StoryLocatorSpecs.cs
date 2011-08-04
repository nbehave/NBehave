using System.IO;
using NBehave.Fluent.Framework.Extensions;
using NBehave.Fluent.Framework.NUnit;
using NBehave.Narrator.Framework;
using NBehave.Spec.NUnit;
using NUnit.Framework;
using Rhino.Mocks;

namespace NBehave.TestDriven.Plugin.Specs
{
    [TestFixture]
    public class StoryLocatorSpecs : ScenarioDrivenSpecBase
    {
        protected override Feature CreateFeature()
        {
            return new Feature("StoryLocator class");
        }

        [Test]
        public void LocateAllStories_should_recurse_subdirectories()
        {
            Feature.AddScenario("Recursing sub-directories")
                   .WithHelperObject<StoryLocatorSpecSteps>()
                   .Given("A StoryLocator")
                   .When("It is asked to locate all story files")
                   .Then("It should recurse sub-directories")
                   ;
        }

        [Test]
        public void LocateStoriesMatching_should_recurse_subdirectories()
        {
            Feature.AddScenario("Recursing sub-directories")
                   .WithHelperObject<StoryLocatorSpecSteps>()
                   .Given("A StoryLocator")
                   .When("It is asked to locate story files matching a type")
                   .Then("It should not recurse sub-directories")
                   ;
        }
    }

    [ActionSteps]
    public class StoryLocatorSpecSteps
    {
        private StoryLocator _storyLocator;
        private IDirectoryWalker _directoryWalker;

        [Given("A StoryLocator")]
        public void SetupStoryLocator()
        {
            _directoryWalker = MockRepository.GenerateStub<IDirectoryWalker>();
            _storyLocator = new StoryLocator
                                {
                                    DirectoryWalker = _directoryWalker,
                                    RootLocation = Path.GetDirectoryName(GetType().Assembly.Location)
                                };
        }

        [When("It is asked to locate all story files")]
        public void LocateAllStories()
        {
            _storyLocator.LocateAllStories();
        }

        [When("It is asked to locate story files matching a type")]
        public void LocateMatchingStories()
        {
            _storyLocator.LocateStoriesMatching(GetType());            
        }

        [Then("It should recurse sub-directories")]
        public void ShouldRecurseSubdirectories()
        {
            _directoryWalker.AssertWasCalled(walker => walker.WalkSubDirectories(null, null), options => options.IgnoreArguments());
        }

        [Then("It should not recurse sub-directories")]
        public void ShouldNotRecurseSubdirectories()
        {
                _directoryWalker.AssertWasNotCalled(walker => walker.WalkSubDirectories(null, null));
        }
    }
}
