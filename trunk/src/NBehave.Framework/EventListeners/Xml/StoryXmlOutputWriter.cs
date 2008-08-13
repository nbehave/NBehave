using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Diagnostics;


namespace NBehave.Narrator.Framework.EventListeners.Xml
{
    public class StoryXmlOutputWriter : XmlOutputBase
    {
        private enum LastCall
        {
            Story,
            StoryMsg,
            Narrative, Narrative_AsA, Narrative_Iwant, Narrative_SoThat,
            Scenario_Title, Scenario_Given_When_Then
        };

        private Timer _currentStoryExecutionTime;
        private string _narrative;
        private LastCall _lastCall = LastCall.Story;
        private string _currentStory = string.Empty;

        private Dictionary<string, ScenarioResults> _scenarioResults = new Dictionary<string, ScenarioResults>();


        public StoryXmlOutputWriter(XmlWriter writer, Queue<Action> actions)
            : base(writer, actions)
        { }


        public void StoryCreated(string story)
        {
            _currentStory = story;
            _scenarioNodeWritten = false;
            _lastCall = LastCall.StoryMsg;
            _currentStoryExecutionTime = new Timer();
            var refToStoryExecutiontime = _currentStoryExecutionTime;
            Actions.Enqueue(
                () =>
                {
                    WriteToStream(refToStoryExecutiontime, "story", story);
                    WriteScenarioResult();
                });
        }

        private Timer _currentScenarioExecutionTime;
        private bool _scenarioNodeWritten = false;

        public void StoryMessageAdded(string message)
        {
            message = message.Trim(new char[] { ' ', '\t' });

            Debug.WriteLine("msg:" + message);
            switch (_lastCall)
            {
                case LastCall.StoryMsg:
                    if (message.StartsWith("Story:"))
                        _lastCall = LastCall.Narrative;
                    break;
                case LastCall.Narrative:
                    if (message.StartsWith("Narrative:"))
                        _lastCall = LastCall.Narrative_AsA;
                    break;
                case LastCall.Narrative_AsA:
                    if (message.StartsWith("As a", StringComparison.CurrentCultureIgnoreCase))
                    {
                        _narrative = message;
                        _lastCall = LastCall.Narrative_Iwant;
                    }
                    break;
                case LastCall.Narrative_Iwant:
                    if (message.StartsWith("I want", StringComparison.CurrentCultureIgnoreCase))
                    {
                        _narrative += Environment.NewLine + message;
                        _lastCall = LastCall.Narrative_SoThat;
                    }
                    break;
                case LastCall.Narrative_SoThat:
                    if (message.StartsWith("So that", StringComparison.CurrentCultureIgnoreCase))
                    {
                        _narrative += Environment.NewLine + message;
                        Actions.Enqueue(() =>
                        {
                            Writer.WriteStartElement("narrative");
                            Writer.WriteString(_narrative);
                            Writer.WriteEndElement();
                            Writer.WriteStartElement("scenarios");
                        });
                        _lastCall = LastCall.Scenario_Title;
                        _scenarioNodeWritten = false;
                    }
                    break;
                case LastCall.Scenario_Title:
                    if (HandleScenarioTitleMessage(message))
                        _lastCall = LastCall.Scenario_Given_When_Then;
                    break;
                case LastCall.Scenario_Given_When_Then:
                    if (IsScenarioTitle(message))
                    {
                        Actions.Enqueue(() =>
                        {
                            Writer.WriteEndElement(); //end previous scenario
                            Writer.WriteEndElement();
                        });
                        _lastCall = LastCall.Scenario_Title;
                        StoryMessageAdded(message);
                    }
                    else
                    {
                        //May be pending too
                        Actions.Enqueue(() =>
                        {
                            Writer.WriteString(message + Environment.NewLine);
                        });
                    }
                    break;
                default:
                    break;
            }
        }

        private bool HandleScenarioTitleMessage(string message)
        {
            bool handled = false;
            if (IsScenarioTitle(message))
            {
                handled = true;
                _scenarioNodeWritten = true;
                string scenarioName = message.Substring(message.IndexOf(":") + 1).Trim();
                _currentScenarioExecutionTime = new Timer();
                var refToScenarioResult = _currentScenarioExecutionTime;
                refToScenarioResult.Stop(); //Right now I dont know how to measure the execution time for a scenario /Morgan
                Actions.Enqueue(() =>
                {
                    WriteToStream(refToScenarioResult, "scenario", scenarioName);
                    Writer.WriteAttributeString("executed", (_scenarioResults[scenarioName].ScenarioResult != ScenarioResult.Pending).ToString().ToLower());
                    Writer.WriteAttributeString("passed", (_scenarioResults[scenarioName].ScenarioResult == ScenarioResult.Passed).ToString().ToLower());
                    Writer.WriteStartElement("text");
                });
            }
            return handled;
        }

        private bool IsScenarioTitle(string message)
        {
            return message.StartsWith("Scenario ");
        }

        public override void DoResults(StoryResults results)
        {
            _currentStoryExecutionTime.Stop();
            IEnumerable<ScenarioResults> resultsForStory = GetScenarioResultsForCurrentStory(results);

            _lastCall = LastCall.Story;
            if (_scenarioNodeWritten)
            {
                Actions.Enqueue(
                    () =>
                    {
                        Writer.WriteEndElement(); // </text>
                        Writer.WriteEndElement(); // </scenario>
                    });
            }

            Actions.Enqueue(
                () =>
                {
                    Writer.WriteEndElement(); // </scenarios>
                    Writer.WriteEndElement(); // </story>
                });
            DoSummaryResult(resultsForStory);
            //This is the end of the story
        }

        private IEnumerable<ScenarioResults> GetScenarioResultsForCurrentStory(StoryResults results)
        {
            var resultsForStory = from res in results.ScenarioResults
                                  where res.StoryTitle == _currentStory
                                  select res;
            foreach (var r in resultsForStory)
                _scenarioResults.Add(r.ScenarioTitle, r);
            return resultsForStory;
        }

        private void DoSummaryResult(IEnumerable<ScenarioResults> resultsForStory)
        {
            StoryResults  rr = new StoryResults();
            foreach (var s in resultsForStory)
                rr.AddResult(s);
            base.DoResults(rr);
        }
    }
}
