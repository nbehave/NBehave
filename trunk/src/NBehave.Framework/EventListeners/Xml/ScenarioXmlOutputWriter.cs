using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace NBehave.Narrator.Framework.EventListeners.Xml
{
    public class ScenarioXmlOutputWriter : XmlOutputBase
    {
        private Timer _currentScenarioExecutionTime;
        private string _currentScenarioTitle;
        private readonly string _belongsToStoryWithTitle;

        public Dictionary<string, ScenarioResults> ScenarioResults { get; private set; }

        public ScenarioXmlOutputWriter(XmlWriter writer, Queue<Action> actions, string belongsToStoryWithTitle)
            : base(writer, actions)
        {
            ScenarioResults = new Dictionary<string, ScenarioResults>();
            _belongsToStoryWithTitle = belongsToStoryWithTitle;
        }

        public void ScenarioCreated(string title)
        {
            _currentScenarioTitle = title;
            _currentScenarioExecutionTime = new Timer();

            var refToScenarioExecutionTime = _currentScenarioExecutionTime;
            refToScenarioExecutionTime.Stop(); //Right now I dont know how to measure the execution time for a scenario /Morgan
            Actions.Enqueue(() =>
            {
                WriteStartElement("scenario", title, refToScenarioExecutionTime);
                Writer.WriteAttributeString("executed", (ScenarioResults[title].ScenarioResult != ScenarioResult.Pending).ToString().ToLower());
                Writer.WriteAttributeString("passed", (ScenarioResults[title].ScenarioResult == ScenarioResult.Passed).ToString().ToLower());
                Writer.WriteStartElement("text");
                while (_messages.Count > 0)
                {
                    _messages.Dequeue().Invoke();
                }
                Writer.WriteEndElement(); // </text>
                Writer.WriteEndElement(); // </scenario>
            });
        }

        private readonly Queue<Action> _messages = new Queue<Action>();

        public void ScenarioMessageAdded(string message)
        {
            _messages.Enqueue(() => Writer.WriteString(message));
        }

        public override void DoResults(StoryResults results)
        {
            var scenarioResults = ExtractResultsForScenario(results);
            base.DoResults(scenarioResults);
        }

        private StoryResults ExtractResultsForScenario(StoryResults results)
        {
            IEnumerable<ScenarioResults> scenarioResults = from r in results.ScenarioResults
                                                           where r.StoryTitle == _belongsToStoryWithTitle
                                                                 && r.ScenarioTitle == _currentScenarioTitle
                                                           select r;
            var storyResults = new StoryResults();
            foreach (var scenarioResult in scenarioResults)
            {
                ScenarioResults.Add(scenarioResult.ScenarioTitle, scenarioResult);
                storyResults.AddResult(scenarioResult);
            }
            return storyResults;
        }
    }
}