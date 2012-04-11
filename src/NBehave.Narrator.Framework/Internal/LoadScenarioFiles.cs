using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NBehave.Narrator.Framework.Internal
{
    public class LoadScenarioFiles
    {
        private readonly NBehaveConfiguration configuration;

        public LoadScenarioFiles(NBehaveConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public IEnumerable<string> LoadFiles()
        {
            IEnumerable<string> files = configuration
                .ScenarioFiles
                .Select(_ => GetFiles(_))
                .SelectMany(_ => _);
            return files.ToList();
        }

        private IEnumerable<string> GetFiles(string location)
        {
            string[] files;
            if (Path.IsPathRooted(location))
            {
                files = Directory.GetFiles(Path.GetDirectoryName(location), Path.GetFileName(location));
            }
            else
            {
                var absoluteLocation = GetAbsolutePath(location);
                var pattern = Path.GetFileName(absoluteLocation);
                var path = Path.GetDirectoryName(absoluteLocation);
                files = Directory.GetFiles(path, pattern, SearchOption.AllDirectories);
            }

            return files;
        }

        private string GetAbsolutePath(string location)
        {
            var directory = Path.GetDirectoryName(location);
            var fileName = Path.GetFileName(location);
            var fullPath = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, directory));
            var fullLocation = Path.Combine(fullPath, fileName);
            return fullLocation;
        }
    }
}