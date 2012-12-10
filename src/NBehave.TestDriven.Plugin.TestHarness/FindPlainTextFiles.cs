using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using NBehave.Attributes;
using NBehave.Contracts;

using NBehave.Spec.NUnit;

namespace NBehave.TestDriven.Plugin.Tests
{
    // This spec is not executed as part of the build - it's here as a test harness where devs can play with TestDriven.Net integration
    [ActionSteps]
    public class FindPlainTextFiles : IMatchFiles, IFileMatcher
    {
        private IEnumerable<string> _storyLocations;
        private Type _type;

        public FindPlainTextFiles()
        {
            Locator = new StoryLocator();
        }

        public StoryLocator Locator { get; set; }


        [Given("an assembly $assemblyname")]
        public void GivenAnAssembly(string assemblyname)
        {
            var assembly = Assembly.Load(assemblyname);
            Locator.RootLocation = Path.GetDirectoryName(assembly.Location);
            _type = null;
        }

        [Given("a class $typename")]
        public void GivenAClass(string typename)
        {
            _type = Type.GetType(typename);
        }

        [When("I look for text files")]
        public void WhenILookForTextFiles()
        {
            _storyLocations = _type == null
                                ? Locator.LocateAllStories()
                                : Locator.LocateStoriesMatching(_type);
        }

        [Then("I should find $number $extension files")]
        public void ThenIShouldFindFiles(int number, string extension)
        {
            var found = from file in _storyLocations
                        where file.EndsWith(extension)
                        select file;

            found.Count().ShouldEqual(number);
        }

        public bool IsMatch(string fileName)
        {
            var fileInfo = new FileInfo(fileName);
            return fileInfo.Directory.Name != "Harness";
        }

        public IFileMatcher FileMatcher
        {
            get { return this; }
        }
    }
}
