using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using gherkin;
using gherkin.lexer;
using java.util;

namespace NBehave.Narrator.Framework
{
    public class GherkinScenarioParser : IScenarioParser, Listener
    {
        private readonly IStringStepRunner _stringStepRunner;
        private readonly char[] _whiteSpaceChars = new[] { ' ', '\t', '\n', '\r' };
        private Feature _feature;
        private readonly List<Feature> _features;
        private ScenarioWithSteps _scenario;
        private ExampleColumns _exampleColumns;
        private bool _midExample;
        private I18n _languageService;
        public const string DefaultLanguage = "en";

        public GherkinScenarioParser(IStringStepRunner stringStepRunner)
        {
            _stringStepRunner = stringStepRunner;
            _scenario = new ScenarioWithSteps(_stringStepRunner);
            _feature = new Feature();
            _scenario.Feature = _feature;
            _feature.AddScenario(_scenario);
            _features = new List<Feature> { _feature };
            _exampleColumns = new ExampleColumns();
        }

        public IEnumerable<Feature> Parse(Stream stream)
        {
            var reader = new StreamReader(stream);
            var scenarioText = reader.ReadToEnd();

            var language = DefaultLanguage;
            var trimmed = scenarioText.TrimStart(_whiteSpaceChars);
            var lang = new Regex(@"^# language:\s+(?<language>\w+)\s+");
            var matches = lang.Match(trimmed);
            if (matches.Success)
            {
                language = matches.Groups["language"].Value;
            }

            _languageService = new I18n(language);

            var lexer = _languageService.lexer(this);
            lexer.scan(scenarioText);

            return _features;
        }

        public void pyString(string content, int line)
        {
        }

        public void feature(string keyword, string title, string description, int line)
        {
            if(_features.Last().HasTitle)
            {
                _feature = new Feature(title);
                _features.Add(_feature);
            }
            else
            {
                _feature.Title = title;                
            }
            _feature.Narrative = description;
        }

        public void background(string keyword, string name, string description, int line)
        {
        }

        public void scenario(string keyword, string title, string description, int line)
        {
            _midExample = false;

            if (!_scenario.Steps.Any())
            {
                _scenario.Feature = _feature;
                _scenario.Title = title;    
            }
            else
            {
                _scenario = new ScenarioWithSteps(_stringStepRunner)
                {
                    Feature = _feature,
                    Title = title
                };

                _feature.AddScenario(_scenario);    
            }
        }

        public void scenarioOutline(string keyword, string name, string description, int line)
        {
        }

        public void examples(string keyword, string name, string description, int line)
        {
            _midExample = true;
        }

        public void step(string keyword, string text, int line)
        {
            _scenario.AddStep(string.Format("{0}{1}", keyword, text));
        }

        public void comment(string comment, int line)
        {
        }

        public void tag(string name, int i)
        {
        }

        public void row(List list, int line)
        {
            if (!_exampleColumns.Any())
            {
                _exampleColumns = new ExampleColumns(list.toArray().Cast<string>().Select(s => s.ToLower()));
            }
            else
            {
                var example = list.toArray().Cast<string>();

                var row = new Dictionary<string, string>();

                for (int i = 0; i < example.Count(); i++)
                {
                    row.Add(_exampleColumns[i], example.ElementAt(i));
                }

                if(_midExample)
                {
                    _scenario.AddExamples(new List<Example> { new Example(_exampleColumns, row) });
                }
                else
                {
                    var step = _scenario.Steps.Last();

                    if(!(step is StringTableStep))
                    {
                        var stringTableStep = new StringTableStep(step.Step, step.Source, _stringStepRunner);
                        _scenario.RemoveLastStep();
                        _scenario.AddStep(stringTableStep);
                    }

                    (_scenario.Steps.Last() as StringTableStep)
                        .AddTableStep(new Row(_exampleColumns, row));
                }
            }
        }

        public void eof()
        {
        }
    }
}