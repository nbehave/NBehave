using System;
using System.Collections.Generic;
using NBehave.Narrator.Framework.EventListeners;

namespace NBehave.Narrator.Framework
{
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

        public void RunScenario(string scenarioText, StoryResults storyResults, int scenarioCounter)
        {
            var scenarioResult = RunScenario(scenarioText, scenarioCounter);
            storyResults.AddResult(scenarioResult);
        }

        private string _storyTitle = string.Empty;
        private string _storyNarrative = string.Empty;

        public ScenarioResult RunScenario(string scenarioText, int scenarioCounter)
        {
            _scenarioEventsToRaise.Clear();
            var scenarioResult = GetScenarioResultInstance(scenarioCounter);
            _textToTokenStringsParser.ParseScenario(scenarioText);

            foreach (var row in _textToTokenStringsParser.TokenStrings)
            {
                HandleStoryTitle(scenarioResult, row);
                HandleStoryNarrative(row);
                HandleScenarioTitle(scenarioResult, row);
                Result stepResult = HandleScenarioStep(row);
                if (stepResult != null)
                {
                    scenarioResult.AddActionStepResult(stepResult);
                    RaiseScenarioMessage(row, stepResult);
                    scenarioResult = UpdateScenarioResult(scenarioResult, stepResult);
                }
            }
            CreateStory();
            var scenario = new Scenario(scenarioResult.ScenarioTitle, Story);
            Story.AddScenario(scenario);
            foreach (var action in _scenarioEventsToRaise)
                action.Invoke();
            return scenarioResult;
        }

        private void CreateStory()
        {
            if (Story == null)
            {
                Story = new Story(_storyTitle);
                Story.Narrative = _storyNarrative;
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
            string scenarioTitle = string.Format("Scenario {0}", scenarioCounter);
            var scenarioResult = new ScenarioResult(_storyTitle, scenarioTitle);
            return scenarioResult;
        }

        private Result HandleScenarioStep(string row)
        {
            Result result = null;
            if (_actionStep.IsScenarioStep(row) && _actionStep.IsScenarioTitle(row) == false)
            {
                result = _actionStepRunner.RunActionStepRow(row);
            }
            return result;
        }

        private void HandleScenarioTitle(ScenarioResult scenarioResult, string row)
        {
            if (_actionStep.IsScenarioTitle(row))
            {
                scenarioResult.ScenarioTitle = _actionStep.GetTitle(row);
                _scenarioEventsToRaise.Enqueue(() => EventListener.ScenarioMessageAdded(row));
            }
        }

        private void HandleStoryNarrative(string row)
        {
            if (_actionStep.IsNarrative(row))
                _storyNarrative += row;
        }

        private void HandleStoryTitle(ScenarioResult scenarioResult, string row)
        {
            if (_actionStep.IsStoryTitle(row))
            {
                _storyTitle = _actionStep.GetTitle(row);
                scenarioResult.StoryTitle = _storyTitle;
                _storyNarrative = string.Empty;
                if (Story != null)
                    Story = null;
            }
        }
    }
}