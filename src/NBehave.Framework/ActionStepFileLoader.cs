// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ActionStepFileLoader.cs" company="NBehave">
//   Copyright (c) 2007, NBehave - http://nbehave.codeplex.com/license
// </copyright>
// <summary>
//   Defines the ActionStepFileLoader type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace NBehave.Narrator.Framework
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using NBehave.Narrator.Framework.Tiny;

    public class ActionStepFileLoader
    {
        private readonly IStringStepRunner _stringStepRunner;

        private readonly ITinyMessengerHub _hub;

        public ActionStepFileLoader(IStringStepRunner stringStepRunner, ITinyMessengerHub hub)
        {
            _stringStepRunner = stringStepRunner;
            _hub = hub;
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
