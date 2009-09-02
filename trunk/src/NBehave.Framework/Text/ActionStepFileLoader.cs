using System.Collections.Generic;
using System.IO;

namespace NBehave.Narrator.Framework
{
    public class ActionStepFileLoader
    {
        private readonly ActionStepAlias _actionStepAlias;
        private readonly ActionStep _actionStep;

        public ActionStepFileLoader(ActionStepAlias actionStepAlias, ActionStep actionStep)
        {
            _actionStepAlias = actionStepAlias;
            _actionStep = actionStep;
        }

        public List<List<ScenarioSteps>> Load(IEnumerable<string> scenarioLocations)
        {
            var stories = new List<List<ScenarioSteps>>();

            foreach (var location in scenarioLocations)
            {
                string[] files = GetFiles(location);
                stories.AddRange(LoadFiles(files));
            }
            return stories;
        }

        private string[] GetFiles(string location)
        {
            string[] files;
            if (Path.IsPathRooted(location))
                files = Directory.GetFiles(Path.GetDirectoryName(location), Path.GetFileName(location));
            else
                files = Directory.GetFiles(".", location);
            return files;
        }

        private List<List<ScenarioSteps>> LoadFiles(IEnumerable<string> files)
        {
            var stories = new List<List<ScenarioSteps>>();
            foreach (var file in files)
            {
                List<ScenarioSteps> scenarios = GetScenarios(file);
                stories.Add(scenarios);
            }
            return stories;
        }

        private List<ScenarioSteps> GetScenarios(string file)
        {
            var scenarios = new List<ScenarioSteps>();
            using (Stream stream = File.OpenRead(file))
            {
                scenarios = Load(stream);
                foreach (var scenario in scenarios)
                    scenario.FileName = file;
            }
            return scenarios;
        }

        public List<ScenarioSteps> Load(Stream stream)
        {
            var scenarioTextParser = new TextToTokenStringsParser(_actionStepAlias, _actionStep);
            using (var fs = new StreamReader(stream))
                scenarioTextParser.ParseScenario(fs.ReadToEnd());
            var tokenStringsToScenarioParser = new TokenStringsToScenarioParser(_actionStep);
            tokenStringsToScenarioParser.ParseTokensToScenarios(scenarioTextParser.TokenStrings);
            List<ScenarioSteps> scenarios = tokenStringsToScenarioParser.Scenarios;

            return scenarios;
        }
    }
}
