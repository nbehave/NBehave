using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NBehave.Narrator.Framework
{
    public class ActionStepFileLoader
    {
        private readonly IStringStepRunner _stringStepRunner;

        public ActionStepFileLoader(IStringStepRunner stringStepRunner)
        {
            _stringStepRunner = stringStepRunner;
        }

        public List<Feature> Load(IEnumerable<string> scenarioLocations)
        {
            var stories = new List<Feature>();

            foreach (var location in scenarioLocations)
            {
                var files = GetFiles(location);
                stories.AddRange(LoadFiles(files));
            }
            return stories;
        }

        private IEnumerable<string> GetFiles(string location)
        {
            string[] files;
            if (Path.IsPathRooted(location))
                files = Directory.GetFiles(Path.GetDirectoryName(location), Path.GetFileName(location));
            else
            {
                var absoluteLocation = GetAbsolutePath(location);
                var path = Path.GetFileName(absoluteLocation);
                var pattern = Path.GetDirectoryName(absoluteLocation);
                files = Directory.GetFiles(pattern, path);
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

        private IEnumerable<Feature> LoadFiles(IEnumerable<string> files)
        {
            var stories = new List<Feature>();
            foreach (var file in files)
            {
                var scenarios = GetScenarios(file);
                stories.AddRange(scenarios);
            }
            return stories;
        }

        private IEnumerable<Feature> GetScenarios(string file)
        {
            IEnumerable<Feature> features;
            using (Stream stream = File.OpenRead(file))
            {
                features = Load(stream);
                foreach (var scenario in features.SelectMany(feature => feature.Scenarios))
                {
                    scenario.Source = file;
                }
            }
            return features;
        }

        private IEnumerable<Feature> Load(Stream stream)
        {
            var scenarioTextParser = new GherkinScenarioParser(_stringStepRunner);
            return scenarioTextParser.Parse(stream);
        }
    }
}
