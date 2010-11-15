// --------------------------------------------------------------------------------------------------------------------
// <copyright file="XmlOutputWriter.cs" company="NBehave">
//   Copyright (c) 2007, NBehave - http://nbehave.codeplex.com/license
// </copyright>
// <summary>
//   Defines the XmlOutputWriter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace NBehave.Narrator.Framework.EventListeners.Xml
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml;

    public class XmlOutputWriter
    {
        public XmlOutputWriter(XmlWriter xmlWriter, IList<EventReceived> eventsReceived)
        {
            Writer = xmlWriter;
            EventsReceived = eventsReceived;
        }

        private XmlWriter Writer { get; set; }

        private IList<EventReceived> EventsReceived { get; set; }

        public void WriteAllXml()
        {
            var evt = (from e in this.EventsReceived
                       where e.EventType == EventType.RunStart
                       select e).First();

            this.Writer.WriteStartElement("results");
            var assemblyString = typeof(XmlOutputEventListener).AssemblyQualifiedName.Split(new[] { ',' });
            this.Writer.WriteAttributeString("name", assemblyString[1]);
            this.Writer.WriteAttributeString("version", assemblyString[2]);
            this.Writer.WriteAttributeString("date", evt.Time.ToShortDateString());
            this.Writer.WriteAttributeString("time", evt.Time.ToShortTimeString());
            this.Writer.WriteAttributeString("themes", this.CountThemes().ToString());
            this.Writer.WriteAttributeString("stories", this.CountStories().ToString());

            this.Writer.WriteAttributeString("scenarios", this.CountScenarios().ToString());
            this.Writer.WriteAttributeString("scenariosFailed", this.CountFailingScenarios().ToString());
            this.Writer.WriteAttributeString("scenariosPending", this.CountPendingScenarios().ToString());

            foreach (var e in this.EventsReceived.Where(e => e.EventType == EventType.ThemeStarted))
            {
                DoTheme(e);
            }

            this.DoRunFinished();
        }

        public void DoTheme(EventReceived evt)
        {
            var events = this.EventsOf(evt, EventType.ThemeFinished);
            var themeTitle = evt.Message;
            this.WriteStartElement("theme", themeTitle, events.Last().Time.Subtract(events.First().Time));
            this.Writer.WriteAttributeString("stories", events.Where(e => e.EventType == EventType.FeatureCreated).Count().ToString());
            this.Writer.WriteAttributeString("scenarios", events.Where(e => e.EventType == EventType.ScenarioCreated).Count().ToString());
            this.Writer.WriteAttributeString("scenariosFailed", CountFailingScenarios(events).ToString());
            this.Writer.WriteAttributeString("scenariosPending", this.CountPendingScenarios(events).ToString());
            this.Writer.WriteStartElement("stories");
            foreach (var e in events.Where(x => x.EventType == EventType.FeatureCreated))
            {
                DoStory(themeTitle, e);
            }

            this.Writer.WriteEndElement();
            this.Writer.WriteEndElement();
        }

        public void DoStory(string theme, EventReceived evt)
        {
            var events = this.EventsOf(evt, EventType.FeatureCreated);
            var featureTitle = evt.Message;
            this.WriteStartElement("story", featureTitle, events.Last().Time.Subtract(events.First().Time));
            var scenarioResultsForFeature = GetScenarioResultsForFeature(featureTitle, events);

            this.WriteStoryDataAttributes(scenarioResultsForFeature);
            this.WriteStoryNarrative(events);
            this.Writer.WriteStartElement("scenarios");
            foreach (var e in events.Where(evts => evts.EventType == EventType.ScenarioCreated))
            {
                var scenarioTitle = e.Message;
                var scenarioResult = (from r in scenarioResultsForFeature
                                      where r.ScenarioTitle == scenarioTitle
                                            && r.FeatureTitle == featureTitle
                                      select r).FirstOrDefault();
                if (scenarioResult != null)
                {
                    DoScenario(e, scenarioResult);
                }
            }

            this.Writer.WriteEndElement();
            this.Writer.WriteEndElement();
        }

        public void DoScenario(EventReceived evt, ScenarioResult scenarioResult)
        {
            var events = from e in this.EventsOf(evt, EventType.ScenarioCreated)
                         where e.EventType == EventType.ScenarioCreated
                         select e;
            this.WriteStartElement("scenario", evt.Message, events.Last().Time.Subtract(events.First().Time));

            this.Writer.WriteAttributeString("outcome", scenarioResult.Result.ToString());
            if (IsPendingAndNoActionStepsResults(scenarioResult))
            {
                this.CreatePendingSteps(evt, scenarioResult);
            }

            foreach (var step in scenarioResult.ActionStepResults)
            {
                DoActionStep(step);
            }

            this.DoExamplesInScenario(scenarioResult as ScenarioExampleResult);
            this.Writer.WriteEndElement();
        }

        public void DoActionStep(ActionStepResult result)
        {
            this.Writer.WriteStartElement("actionStep");
            this.Writer.WriteAttributeString("name", result.StringStep);
            this.Writer.WriteAttributeString("outcome", result.Result.ToString());
            if (result.Result.GetType() == typeof(Failed))
            {
                this.Writer.WriteElementString("failure", result.Message);
            }

            this.Writer.WriteEndElement();
        }

        private void DoRunFinished()
        {
            this.Writer.WriteEndElement(); // </results>
            this.Writer.Flush();
        }

        private void DoExamplesInScenario(ScenarioExampleResult scenarioExampleResult)
        {
            if (scenarioExampleResult == null)
            {
                return;
            }

            this.Writer.WriteStartElement("examples");
            this.Writer.WriteStartElement("columnNames");

            foreach (var columnName in scenarioExampleResult.Examples.First().ColumnNames)
            {
                this.Writer.WriteStartElement("columnName");
                this.Writer.WriteString(columnName);
                this.Writer.WriteEndElement();
            }

            this.Writer.WriteEndElement();

            var scenarioResults = scenarioExampleResult.ExampleResults.ToArray();
            var idx = 0;
            foreach (var example in scenarioExampleResult.Examples)
            {
                this.Writer.WriteStartElement("example");
                this.Writer.WriteStartAttribute("outcome");
                this.Writer.WriteString(scenarioResults[idx++].Result.ToString());
                this.Writer.WriteEndAttribute();
                foreach (var columnName in example.ColumnNames)
                {
                    this.Writer.WriteStartElement("column");
                    this.Writer.WriteStartAttribute("columnName");
                    this.Writer.WriteString(columnName);
                    this.Writer.WriteEndAttribute();
                    this.Writer.WriteString(example.ColumnValues[columnName]);
                    this.Writer.WriteEndElement();
                }

                this.Writer.WriteEndElement();
            }

            this.Writer.WriteEndElement();
        }

        private IEnumerable<ScenarioResult> GetScenarioResultsForFeature(string featureTitle, IEnumerable<EventReceived> eventsReceived)
        {
            var featureResults = from e in eventsReceived
                                  where e.EventType == EventType.ScenarioResult
                                  select e as ScenarioResultEventReceived;
            var scenarioResultsForFeature = from e in featureResults
                                            where e.ScenarioResult.FeatureTitle == featureTitle
                                            select e.ScenarioResult;
            return scenarioResultsForFeature;
        }

        private IEnumerable<ScenarioResult> GetScenarioResults(IEnumerable<EventReceived> events, Predicate<Result> scenarioResult)
        {
            var storyResults = from e in events
                               where e.EventType == EventType.ScenarioResult
                               select e as ScenarioResultEventReceived;
            var eventsToUse = events.Where(e => e.EventType == EventType.ScenarioCreated);
            var sr = from s in storyResults
                     where HasScenario(eventsToUse, s.ScenarioResult.ScenarioTitle)
                           && scenarioResult(s.ScenarioResult.Result)
                     select s.ScenarioResult;
            return sr;
        }

        private bool IsPendingAndNoActionStepsResults(ScenarioResult scenarioResult)
        {
            return scenarioResult.Result.GetType() == typeof(Pending) && (scenarioResult.ActionStepResults.Count() == 0);
        }

        private void WriteStoryNarrative(IEnumerable<EventReceived> events)
        {
            var featureMessages = from m in events
                                  where m.EventType == EventType.FeatureNarrative
                                  select m.Message;
            if (featureMessages.Count() > 0)
            {
                Writer.WriteStartElement("narrative");
                foreach (var row in featureMessages)
                {
                    Writer.WriteString(row + Environment.NewLine);
                }

                Writer.WriteEndElement();
            }
        }

        private void WriteStoryDataAttributes(IEnumerable<ScenarioResult> scenarioResultsForFeature)
        {
            var totalScenariosFailed = (from f in scenarioResultsForFeature
                                        where f.Result.GetType() == typeof(Failed)
                                        select f).Count();
            var totalScenariosPending = (from f in scenarioResultsForFeature
                                         where f.Result.GetType() == typeof(Pending)
                                         select f).Count();
            Writer.WriteAttributeString("scenarios", scenarioResultsForFeature.Count().ToString());
            Writer.WriteAttributeString("scenariosFailed", totalScenariosFailed.ToString());
            Writer.WriteAttributeString("scenariosPending", totalScenariosPending.ToString());
        }

        private void WriteStartElement(string elementName, string attributeName, TimeSpan timeTaken)
        {
            this.Writer.WriteStartElement(elementName);
            this.Writer.WriteAttributeString("name", attributeName);
            this.Writer.WriteAttributeString("time", timeTaken.TotalSeconds.ToString());
        }

        private void CreatePendingSteps(EventReceived evt, ScenarioResult scenarioResult)
        {
            var actionSteps = from e in EventsOf(evt, EventType.ScenarioResult)
                              where e.EventType == EventType.ScenarioCreated
                              select e;
            foreach (var step in actionSteps)
            {
                scenarioResult.AddActionStepResult(
                    new ActionStepResult(step.Message, new Pending(scenarioResult.Message)));
            }
        }

        private int CountThemes()
        {
            return CountEventsOfType(EventType.ThemeStarted);
        }

        private int CountStories()
        {
            return CountEventsOfType(EventType.FeatureCreated);
        }

        private int CountScenarios()
        {
            var storyResults = GetScenarioResults(this.EventsReceived, p => true);
            return storyResults.Count();
        }

        private int CountFailingScenarios()
        {
            return CountFailingScenarios(this.EventsReceived);
        }

        private int CountFailingScenarios(IEnumerable<EventReceived> events)
        {
            var scenarioResults = GetScenarioResults(events, s => s.GetType() == typeof(Failed));
            return scenarioResults.Count();
        }

        private int CountPendingScenarios()
        {
            return this.CountPendingScenarios(this.EventsReceived);
        }

        private int CountPendingScenarios(IEnumerable<EventReceived> events)
        {
            var scenarioResults = this.GetScenarioResults(events, s => s.GetType() == typeof(Pending));

            return scenarioResults.Count();
        }

        private int CountEventsOfType(EventType eventType)
        {
            var themes = from e in this.EventsReceived
                         where e.EventType == eventType
                         select e;
            return themes.Count();
        }

        private bool HasScenario(IEnumerable<EventReceived> eventsToUse, string scenarioTitle)
        {
            var scenario = from s in eventsToUse
                           where s.Message == scenarioTitle
                           select s;
            return scenario.Count() > 0;
        }

        private IEnumerable<EventReceived> EventsOf(EventReceived startEvent, EventType endWithEvent)
        {
            var idxStart = this.EventsReceived.IndexOf(startEvent);
            var idxEnd = idxStart;
            var events = new List<EventReceived>();
            do
            {
                events.Add(this.EventsReceived[idxEnd]);
                idxEnd++;
            }
            while (idxEnd < this.EventsReceived.Count && this.EventsReceived[idxEnd].EventType != endWithEvent);
            if (idxEnd < this.EventsReceived.Count)
            {
                events.Add(this.EventsReceived[idxEnd]);
            }

            return events;
        }
    }
}