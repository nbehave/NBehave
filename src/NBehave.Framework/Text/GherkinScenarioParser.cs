using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Gherkin;

namespace NBehave.Narrator.Framework
{
    public class GherkinScenarioParser : IListener
    {
        private readonly IStringStepRunner _stringStepRunner;
        private Feature _feature;
        private readonly List<Feature> _features;
        private ScenarioWithSteps _scenario;
        private ExampleColumns _exampleColumns;
        private bool _midExample;
        private readonly LanguageService _languageService;

        public GherkinScenarioParser(IStringStepRunner stringStepRunner)
        {
            _stringStepRunner = stringStepRunner;
            _languageService = new LanguageService();
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

            var lexer = _languageService.GetLexer(scenarioText, this);
            lexer.Scan(reader);

            return _features;
        }


        public void Feature(Token keyword, Token title)
        {
            if(_features.Last().HasTitle)
            {
                _feature = new Feature(title.Content);
                _features.Add(_feature);
            }
            else
            {
                _feature.Title = title.Content;                
            }
        }

        public void Scenario(Token keyword, Token title)
        {
            _midExample = false;

            if (!_scenario.Steps.Any())
            {
                _scenario.Feature = _feature;
                _scenario.Title = title.Content;    
            }
            else
            {
                _scenario = new ScenarioWithSteps(_stringStepRunner)
                {
                    Feature = _feature,
                    Title = title.Content
                };

                _feature.AddScenario(_scenario);    
            }
        }

        public void Examples(Token keyword, Token name)
        {
            _midExample = true;
        }

        public void Step(Token keyword, Token name, StepKind stepKind)
        {
            _scenario.AddStep(string.Format("{0}{1}", keyword, name.Content));
        }

        public void Table(IList<IList<Token>> rows, Position tablePosition)
        {
        }

        public void Row(ArrayList list, int line)
        {
            if (!_exampleColumns.Any())
            {
                _exampleColumns = new ExampleColumns(list.ToArray().Cast<string>().Select(s => s.ToLower()));
            }
            else
            {
                var example = list.ToArray().Cast<string>();

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

        public void Background(Token keyword, Token name)
        {
        }

        public void ScenarioOutline(Token keyword, Token name)
        {
        }

        public void Comment(Token comment)
        {
        }

        public void Tag(Token name)
        {
        }

        public void PythonString(Token content)
        {
        }

        public void SyntaxError(string state, string @event, IEnumerable<string> legalEvents, Position position)
        {
        }

        public void Eof()
        {
        }
    }
}