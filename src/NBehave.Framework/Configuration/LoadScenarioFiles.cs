namespace NBehave.Narrator.Framework.Contracts
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using NBehave.Narrator.Framework.Messages;
    using NBehave.Narrator.Framework.Tiny;

    public class LoadScenarioFiles : IStartupTask
    {
        private readonly NBehaveConfiguration _configuration;
        private readonly ITinyMessengerHub _hub;
        private readonly IStringStepRunner _stringStepRunner;

        public LoadScenarioFiles(NBehaveConfiguration configuration, ITinyMessengerHub hub, IStringStepRunner stringStepRunner)
        {
            this._configuration = configuration;
            _hub = hub;
            this._stringStepRunner = stringStepRunner;
        }

        public void Initialise()
        {
            var features = this.GetFeatures();
            this._hub.Publish(new FeaturesLoaded(this, features));
        }

        private IEnumerable<Feature> GetFeatures()
        {
            return Load(this._configuration.ScenarioFiles);
        }

        public List<Feature> Load(IEnumerable<string> scenarioLocations)
        {
            var stories = new List<Feature>();

            foreach (var location in scenarioLocations)
            {
                var files = GetFiles(location);
                IEnumerable<Feature> loadFiles = this.LoadFiles(files);
                stories.AddRange(loadFiles);
            }

            return stories;
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
                var scenarioTextParser = new GherkinScenarioParser(this._stringStepRunner, this._hub);
                features = scenarioTextParser.Parse(stream);

                foreach (var scenario in features.SelectMany(feature => feature.Scenarios))
                {
                    scenario.Source = file;
                }
            }

            return features;
        }
    }
}