using System;
using System.Collections.Generic;
using System.IO;

namespace NBehave.Narrator.Framework
{
    public class ActionStepFileLoader
    {
        private readonly ActionStepAlias _actionStepAlias;

        public ActionStepFileLoader(ActionStepAlias actionStepAlias)
        {
            _actionStepAlias = actionStepAlias;
        }

        public List<string> Load(IEnumerable<string> scenarioLocations)
        {
            var scenarios = new List<string>();

            foreach (var location in scenarioLocations)
            {
                string[] files;
                if (Path.IsPathRooted(location))
                    files = Directory.GetFiles(Path.GetDirectoryName(location), Path.GetFileName(location));
                else
                    files = Directory.GetFiles(".", location);
                foreach (var file in files)
                {
                    Stream stream = File.OpenRead(file);
                    scenarios.AddRange(Load(stream));
                }
            }
            return scenarios;
        }

        public List<string> Load(Stream stream)
        {
            var scenarioTextParser = new TextToTokenStringsParser(_actionStepAlias);
            using (var fs = new StreamReader(stream))
                scenarioTextParser.ParseScenario(fs.ReadToEnd());
            var tokenStringsToScenarioParser = new TokenStringsToScenarioParser();
            tokenStringsToScenarioParser.ParseTokensToScenarios(scenarioTextParser.TokenStrings);
            List<string> scenarios = tokenStringsToScenarioParser.Scenarios;

            return scenarios;
        }
    }
}
