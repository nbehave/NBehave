using System;
using System.Collections.Generic;
using System.IO;
using NBehave.Narrator.Framework.EventListeners;

namespace NBehave.Narrator.Framework
{
    public class ScenarioSteps
    {
        public string FileName { get; set; }
        public string Steps { get; set; }
    }

    public class ScenarioStepRunner
    {
        private readonly ActionStep _actionStep;
        private readonly TextToTokenStringsParser _textToTokenStringsParser;
        private readonly ActionStepRunner _actionStepRunner;

        public IEventListener EventListener { get; set; }
        private Story Story { get; set; }
        private readonly Queue<Action> _scenarioEventsToRaise = new Queue<Action>();

        public ScenarioStepRunner(TextToTokenStringsParser textToTokenStringsParser,
                                    ActionStepRunner actionStepRunner,
                                    ActionStep actionStep)
        {
            _textToTokenStringsParser = textToTokenStringsParser;
            _actionStepRunner = actionStepRunner;
            _actionStep = actionStep;
            EventListener = new NullEventListener();
        }

        public void RunScenarios(IEnumerable<ScenarioSteps> scenarios, StoryResults storyResults)
        {
            int scenarioCounter = 1;
            foreach (var scenario in scenarios)
            {
                var scenarioResult = RunScenario(scenario, scenarioCounter);
                storyResults.AddResult(scenarioResult);
                scenarioCounter++;
            }
        }

        private string _storyTitle = string.Empty;
        private string _storyNarrative = string.Empty;

        private ScenarioResult _scenarioResult;
        private ScenarioSteps _scenarioSteps;
        public ScenarioResult RunScenario(ScenarioSteps scenarioSteps, int scenarioCounter)
        {
            _scenarioSteps = scenarioSteps;
            _textToTokenStringsParser.ParseScenario(scenarioSteps.Steps);

            foreach (var row in _textToTokenStringsParser.TokenStrings)
            {
                HandleStoryTitle(row);
                HandleStoryNarrative(row);
                HandleScenarioTitle(row);
                HandleScenarioStep(row, scenarioCounter);

            }
            CreateStoryIfStoryNull();
            var scenario = new Scenario(_scenarioResult.ScenarioTitle, Story);
            Story.AddScenario(scenario);
            foreach (var action in _scenarioEventsToRaise)
                action.Invoke();
            return _scenarioResult;
        }

        private void SetStoryNarrative()
        {
            if (string.IsNullOrEmpty(Story.Narrative) && string.IsNullOrEmpty(_storyNarrative) == false)
            {
                Story.Narrative = _storyNarrative;
                Story.OnMessageAdded(this, new EventArgs<MessageEventData>(new MessageEventData("Narrative", _storyNarrative)));
            }
        }

        private void CreateStoryIfStoryNull()
        {
            if (Story == null)
            {
                Story = new Story(_storyTitle);
                Story.Narrative = _storyNarrative;
                foreach (var narrativeRow in _storyNarrative.Replace(Environment.NewLine, '\n'.ToString()).Split(new[] { '\n' }))
                    EventListener.StoryMessageAdded(narrativeRow);
            }
        }

        private ScenarioResult UpdateScenarioResult(ScenarioResult scenarioResult, Result stepResult)
        {
            if (stepResult.GetType() == typeof(Pending))
                scenarioResult.Pend(stepResult.Message);
            if (stepResult.GetType() == typeof(Failed))
                scenarioResult.Fail((stepResult as Failed).Exception);
            return scenarioResult;
        }

        private void RaiseScenarioMessage(string row, Result result)
        {
            if (result.GetType() == typeof(Passed))
                _scenarioEventsToRaise.Enqueue(() => EventListener.ScenarioMessageAdded(row));
            else
                _scenarioEventsToRaise.Enqueue(() => EventListener.ScenarioMessageAdded(row + " - " + result.ToString().ToUpper()));
        }

        private ScenarioResult GetScenarioResultInstance(int scenarioCounter)
        {
            var scenarioTitle = string.Format("{0}.{1}", scenarioCounter, Path.GetFileNameWithoutExtension(_scenarioSteps.FileName));
            var scenarioResult = new ScenarioResult(Story, scenarioTitle);
            return scenarioResult;
        }

        private void HandleScenarioStep(string row, int scenarioCounter)
        {
            Result result = null;

            if (_actionStep.IsScenarioStep(row) && _actionStep.IsScenarioTitle(row) == false)
            {
                CreateStoryIfStoryNull();
                if (_scenarioResult == null)
                    _scenarioResult = GetScenarioResultInstance(scenarioCounter);
                result = _actionStepRunner.RunActionStepRow(row);
            }
            if (result != null)
            {
                SetStoryNarrative();
                _scenarioResult.AddActionStepResult(result);
                RaiseScenarioMessage(row, result);
                _scenarioResult = UpdateScenarioResult(_scenarioResult, result);
            }
        }

        private void HandleScenarioTitle(string row)
        {
            if (_actionStep.IsScenarioTitle(row))
            {
                CreateStoryIfStoryNull();
                _scenarioResult = new ScenarioResult(Story, _actionStep.GetTitle(row));
            }
        }

        private void HandleStoryNarrative(string row)
        {
            if (_actionStep.IsNarrative(row))
            {
                if (string.IsNullOrEmpty(_storyNarrative))
                    _storyNarrative += row;
                else
                    _storyNarrative += Environment.NewLine + row;
            }
        }

        private void HandleStoryTitle(string row)
        {
            if (_actionStep.IsStoryTitle(row))
            {
                _storyTitle = _actionStep.GetTitle(row);
                _storyNarrative = string.Empty;
                Story = null;
            }
        }
    }
}