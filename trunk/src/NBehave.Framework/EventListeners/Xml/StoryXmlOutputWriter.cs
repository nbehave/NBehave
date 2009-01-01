using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;


namespace NBehave.Narrator.Framework.EventListeners.Xml
{
    public class StoryXmlOutputWriter : XmlOutputBase
    {
        private Timer _currentStoryExecutionTime;
        private string _narrative = string.Empty;
        private string _currentStory = string.Empty;

        public StoryXmlOutputWriter(XmlWriter writer, Queue<Action> actions)
            : base(writer, actions)
        { }

        public string CurrentStoryTitle
        {
            get { return _currentStory; }
        }

        public void StoryCreated(string story)
        {
            _currentStory = story;
            _currentStoryExecutionTime = new Timer();
            var refToStoryExecutiontime = _currentStoryExecutionTime;
            Actions.Enqueue(
                () =>
                {
                    WriteStartElement("story", story, refToStoryExecutiontime);
                    WriteScenarioResult();
                });
        }

        public void StoryMessageAdded(string message)
        {
            message = message.Trim(new[] { ' ', '\t' });

            _narrative += message + Environment.NewLine;

            if (message.StartsWith("So that", StringComparison.CurrentCultureIgnoreCase))
            {
                string narrativeToWrite = _narrative;
                Actions.Enqueue(() =>
                {
                    Writer.WriteStartElement("narrative");
                    Writer.WriteString(narrativeToWrite);
                    Writer.WriteEndElement();
                });
                _narrative = string.Empty;
            }
        }

        public override void DoResults(StoryResults results)
        {
            _currentStoryExecutionTime.Stop();
            IEnumerable<ScenarioResults> resultsForStory = GetScenarioResultsForCurrentStory(results);           
            DoSummaryResult(resultsForStory);
        }

        private IEnumerable<ScenarioResults> GetScenarioResultsForCurrentStory(StoryResults results)
        {
            var resultsForStory = from res in results.ScenarioResults
                                  where res.StoryTitle == _currentStory
                                  select res;
            return resultsForStory;
        }

        private void DoSummaryResult(IEnumerable<ScenarioResults> resultsForStory)
        {
            Actions.Enqueue(()=> Writer.WriteEndElement());
            var storyResults = new StoryResults();
            foreach (var s in resultsForStory)
                storyResults.AddResult(s);
            base.DoResults(storyResults);
        }
    }
}
