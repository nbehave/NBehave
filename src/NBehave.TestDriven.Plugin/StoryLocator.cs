using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NBehave.Narrator.Framework;

namespace NBehave.TestDriven.Plugin
{
    public class StoryLocator : IDirectoryVisitor
    {
        private readonly List<string> _locatedStories = new List<string>();
        private static readonly string[] Extensions = NBehaveConfiguration.FeatureFileExtensions.Select(_ => "*" + _).ToArray();
        private IDirectoryWalker _directoryWalker = new DirectoryWalker();

        public string RootLocation { get; set; }

        public IDirectoryWalker DirectoryWalker
        {
            get { return _directoryWalker; }
            set { _directoryWalker = value; }
        }

        public IEnumerable<string> LocateAllStories()
        {
            return ProcessDirectories(RootLocation);
        }

        public IEnumerable<string> LocateStoriesMatching(Type type)
        {
            if (type == null)
                return LocateAllStories();

            var rootNamespace = AssemblyHelper.DeduceRootNamespaceParts(type.Assembly)
                                              .ToArray();
            var navigationToMember = type.Namespace
                                           .Split('.')
                                           .SkipWhile((part, i) => i < rootNamespace.Length && rootNamespace[i] == part);

            var subdirectory = navigationToMember.Aggregate(RootLocation, Path.Combine);

            return ProcessDirectory(subdirectory)
                        .Where(storyFile => Path.GetFileNameWithoutExtension(storyFile) == type.Name);
        }

        private IEnumerable<string> ProcessDirectories(string startDirectory)
        {
            _locatedStories.Clear();

            if (Directory.Exists(startDirectory))
            {
                DirectoryWalker.WalkSubDirectories(startDirectory, this);
            }

            return _locatedStories;
        }

        private IEnumerable<string> ProcessDirectory(string directory)
        {
            _locatedStories.Clear();

            if (Directory.Exists(directory))
            {
                VisitDirectory(new DirectoryInfo(directory));
            }

            return _locatedStories;
        }

        public void VisitDirectory(DirectoryInfo directory)
        {
            _locatedStories.AddRange(directory.GetFiles()
                                              .AsEnumerable()
                                              .Where(file => Extensions.Contains(file.Extension))
                                              .Select(file => file.FullName));
        }
    }
}
