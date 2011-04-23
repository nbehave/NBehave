using System;

namespace NBehave.Narrator.Framework
{
    public class XmlToConsoleOutputEventListener : IEventListener
    {
        private readonly bool _showResults;
        private bool isStoryOpen;

        public XmlToConsoleOutputEventListener(bool showResults)
        {
            _showResults = showResults;
        }

        public void FeatureCreated(string feature)
        {
            Console.WriteLine("<feature><title>" + feature + "</title>");
        }

        public void FeatureNarrative(string message)
        {
            Console.WriteLine("<story><title>" + message + "</title>");
            isStoryOpen = true;
        }

        public void ScenarioCreated(string scenarioTitle)
        {
        }

        public void RunStarted()
        {
        }

        public void RunFinished()
        {
        }

        public void ThemeStarted(string name)
        {
        }

        public void ThemeFinished()
        {
        }

        public void ScenarioResult(ScenarioResult scenarioResult)
        {
            Console.WriteLine("<scenario><title>" + scenarioResult.ScenarioTitle + (_showResults ? " - " + scenarioResult.Result.ToString().ToUpper() : "") + "</title>");
            foreach (var stepResult in scenarioResult.ActionStepResults)
                Console.WriteLine("<step>" + stepResult.StringStep + (_showResults ? " - " + stepResult.Result.ToString().ToUpper() : "") + "</step>");
            Console.WriteLine("</scenario>" + (isStoryOpen ? "</story>" : "") + "</feature>");

        }
    }
}