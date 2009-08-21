using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace NBehave.Narrator.Framework
{
    public class ActionMethodInfo : ActionMatch
    {
        public string ActionType { get; set; } //Given, When Then etc..
        public MethodInfo MethodInfo { get; set; }
    }

    public class ActionStepRunner : RunnerBase
    {
        private readonly List<string> _scenarios = new List<string>();
        private readonly ActionStepAlias _actionStepAlias = new ActionStepAlias();
        private readonly ActionStep _actionStep;
        private readonly ActionStepFileLoader _actionStepFileLoader;

        public ActionCatalog ActionCatalog { get; private set; }

        public ActionStepRunner()
        {
            ActionCatalog = new ActionCatalog();
            StoryRunnerFilter = new StoryRunnerFilter();
            _actionStep = new ActionStep(_actionStepAlias);
            _actionStepFileLoader = new ActionStepFileLoader(_actionStepAlias, _actionStep);
        }

        protected override void ParseAssembly(Assembly assembly)
        {
            ActionStepParser parser = new ActionStepParser(StoryRunnerFilter, ActionCatalog, _actionStepAlias);
            parser.FindActionSteps(assembly);
        }

        protected override void RunStories(StoryResults results, IEventListener listener)
        {
            listener.ThemeStarted(string.Empty);
            RunScenarios(results, listener);
            listener.StoryResults(results);
            listener.ThemeFinished();
            ClearStoryList();
        }

        private void RunScenarios(StoryResults storyResults, IEventListener listener)
        {
            int scenarioCounter = 0;
            foreach (string scenarioText in _scenarios)
            {
                scenarioCounter++;
                RunScenario(scenarioText, storyResults, listener, scenarioCounter);
            }
        }

        private void RunScenario(string scenarioText, StoryResults storyResults, IEventListener listener,
                                 int scenarioCounter)
        {
            Story story = null;
            var textToTokenStringsParser = new TextToTokenStringsParser(_actionStepAlias, _actionStep);

            textToTokenStringsParser.ParseScenario(scenarioText);
            string scenarioTitle = string.Format("Scenario {0}", scenarioCounter);
            var scenarioResult = new ScenarioResults(string.Empty, scenarioTitle);

            string scenarioMessageToAdd = string.Empty;
            foreach (var row in textToTokenStringsParser.TokenStrings)
            {
                if (_actionStep.IsStoryTitle(row))
                {
                    if (story == null)
                        story = new Story(_actionStep.GetTitle(row));
                    scenarioResult.StoryTitle = story.Title;
                }
                else if (_actionStep.IsNarrative(row))
                {
                    if (story == null)
                        story = new Story(string.Empty);
                    story.Narrative += row;
                }
                else if (_actionStep.IsScenarioTitle(row))
                    scenarioResult.ScenarioTitle = _actionStep.GetTitle(row);
                else
                {
                    ScenarioResult result = RunActionStepRow(row, scenarioResult);
                    if (result == ScenarioResult.Passed)
                        scenarioMessageToAdd += row + Environment.NewLine;
                    else
                        scenarioMessageToAdd += row + " - " + result.ToString().ToUpper() + Environment.NewLine;
                }
            }
            if (story == null)
                story = new Story(string.Empty);
            var scenario = new Scenario(scenarioResult.ScenarioTitle, story);
            story.AddScenario(scenario);
            listener.ScenarioMessageAdded(scenarioMessageToAdd);
            storyResults.AddResult(scenarioResult);
        }

        private ScenarioResult RunActionStepRow(string row, ScenarioResults scenarioResult)
        {
            ScenarioResult result = ScenarioResult.Passed;
            try
            {
                string rowWithoutActionType = row.RemoveFirstWord();
                if (ActionCatalog.ActionExists(rowWithoutActionType) == false)
                {
                    scenarioResult.Pend(string.Format("No matching Action found for \"{0}\"", row));
                    result = ScenarioResult.Pending;
                }
                else
                    InvokeTokenString(rowWithoutActionType);
            }
            catch (Exception e)
            {
                result = ScenarioResult.Failed;
                Exception realException = FindUsefulException(e);
                scenarioResult.Fail(realException);
            }
            return result;
        }

        private Exception FindUsefulException(Exception e)
        {
            Exception realException = e;
            while (realException != null && realException.GetType() == typeof(TargetInvocationException))
            {
                realException = realException.InnerException;
            }
            if (realException == null)
                return e;
            return realException;
        }

        public void InvokeTokenString(string tokenString)
        {
            if (ActionCatalog.ActionExists(tokenString) == false)
                throw new ArgumentException(string.Format("cannot find Token string '{0}'", tokenString));

            object action = ActionCatalog.GetAction(tokenString).Action;

            Type actionType = action.GetType().IsGenericType
                ? action.GetType().GetGenericTypeDefinition()
                : action.GetType();
            MethodInfo methodInfo = actionType.GetMethod("DynamicInvoke");
            object[] actionParamValues = ActionCatalog.GetParametersForMessage(tokenString);

            methodInfo.Invoke(action, BindingFlags.InvokeMethod, null,
                              new object[] { actionParamValues }, CultureInfo.CurrentCulture);
        }

        public void Load(IEnumerable<string> scenarioLocations)
        {
            var scenarios = _actionStepFileLoader.Load(scenarioLocations);
            _scenarios.AddRange(scenarios);
        }

        public void Load(Stream stream)
        {
            var scenarios = _actionStepFileLoader.Load(stream);
            _scenarios.AddRange(scenarios);
        }

        private void InvokeActionBehaviour(string actionStep)
        {

        }
    }
}