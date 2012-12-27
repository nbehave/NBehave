// --------------------------------------------------------------------------------------------------------------------
// <copyright file="XmlOutputWriter.cs" company="NBehave">
//   Copyright (c) 2007, NBehave - http://nbehave.codeplex.com/license
// </copyright>
// <summary>
//   Defines the XmlOutputWriter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace NBehave.EventListeners.Xml
{
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
            var evt = EventsReceived.OrderBy(_ => _.Time).FirstOrDefault();

            Writer.WriteStartElement("results");
            var semVer = (AssemblyInformationalVersionAttribute)
             Attribute.GetCustomAttribute(GetType().Assembly, typeof(AssemblyInformationalVersionAttribute));

            Writer.WriteAttributeString("name", "NBehave");
            Writer.WriteAttributeString("version", semVer.InformationalVersion);
            Writer.WriteAttributeString("date", evt.Time.ToShortDateString());
            Writer.WriteAttributeString("time", evt.Time.ToShortTimeString());
            Writer.WriteAttributeString("executionTime", Math.Round(CalcExecutionTime().TotalSeconds, 1).ToString());
            Writer.WriteAttributeString("features", CountFeatures().ToString());

            Writer.WriteAttributeString("scenarios", CountScenarios().ToString());
            Writer.WriteAttributeString("scenariosFailed", CountFailingScenarios().ToString());
            Writer.WriteAttributeString("scenariosPending", CountPendingScenarios().ToString());

            DoFeatures();
            DoRunFinished();
        }

        private TimeSpan CalcExecutionTime()
        {
            var start = (EventsReceived.OrderBy(_ => _.Time)).First();
            var end = (EventsReceived.OrderBy(_ => _.Time)).Last();
            return end.Time.Subtract(start.Time);
        }

        private void DoFeatures()
        {
            Writer.WriteStartElement("features");
            foreach (var e in EventsReceived.Where(e => e.EventType == EventType.FeatureStart))
                DoFeature(e);
            Writer.WriteEndElement();
        }

        public void DoFeature(EventReceived evt)
        {
            var events = EventsOf(evt, EventType.FeatureStart);
            var featureTitle = evt.Message;
            WriteStartElement("feature", featureTitle, events.Last().Time.Subtract(events.First().Time));
            var scenarioResultsForFeature = GetScenarioResultsForFeature(featureTitle, events);

            WriteStoryDataAttributes(scenarioResultsForFeature);
            WriteStoryNarrative(events);
            WriteBackgroundEvents(events);
            WriteScenarioEvents(scenarioResultsForFeature, featureTitle, events);
            Writer.WriteEndElement();
        }

        private void WriteScenarioEvents(IEnumerable<ScenarioResult> scenarioResultsForFeature, string featureTitle, IEnumerable<EventReceived> events)
        {
            Writer.WriteStartElement("scenarios");
            foreach (var e in events.Where(evts => evts.EventType == EventType.ScenarioStart))
            {
                var scenarioTitle = e.Message;
                var scenarioResult = (from r in scenarioResultsForFeature
                                      where r.ScenarioTitle == scenarioTitle
                                            && r.FeatureTitle == featureTitle
                                      select r).FirstOrDefault();
                if (scenarioResult != null)
                    DoScenario(e, scenarioResult);
            }

            Writer.WriteEndElement();
        }

        private void WriteBackgroundEvents(IEnumerable<EventReceived> events)
        {
            var scenarioResultEvent = events
                .Where(_ => _ is ScenarioResultEventReceived)
                .Cast<ScenarioResultEventReceived>()
                .FirstOrDefault();
            if (scenarioResultEvent != null && scenarioResultEvent.ScenarioResult.HasBackgroundResults())
            {
                DoBackground(scenarioResultEvent.ScenarioResult);
            }
        }

        public void DoBackground(ScenarioResult scenarioResult)
        {
            Writer.WriteStartElement("background");
            WriteBackgroundSteps(scenarioResult);
            Writer.WriteEndElement();
        }

        private void WriteBackgroundSteps(ScenarioResult scenarioResult)
        {
            var bgSteps = scenarioResult.StepResults
                .Where(_ => _ is BackgroundStepResult)
                .Cast<BackgroundStepResult>()
                .ToList();
            var backgroundTitle = (bgSteps.Any()) ? bgSteps.First().BackgroundTitle : "";
            Writer.WriteAttributeString("name", backgroundTitle);
            string outcome = "passed";
            outcome = (bgSteps.Any(_ => _.Result is Pending)) ? "pending" : outcome;
            outcome = (bgSteps.Any(_ => _.Result is Failed)) ? "pending" : outcome;
            Writer.WriteAttributeString("outcome", outcome);

            foreach (var step in bgSteps)
                DoStringStep(step);
        }

        public void DoScenario(EventReceived evt, ScenarioResult scenarioResult)
        {
            var events = from e in EventsOf(evt, EventType.ScenarioStart)
                         where e.EventType == EventType.ScenarioStart
                         select e;
            WriteStartElement("scenario", evt.Message, events.Last().Time.Subtract(events.First().Time));

            Writer.WriteAttributeString("outcome", scenarioResult.Result.ToString());

            if (IsPendingAndNoActionStepsResults(scenarioResult))
                CreatePendingSteps(evt, scenarioResult);

            foreach (var step in scenarioResult.StepResults)
                DoStringStep(step);

            DoExamplesInScenario(scenarioResult as ScenarioExampleResult);
            Writer.WriteEndElement();
        }

        public void DoStringStep(StepResult result)
        {
            Writer.WriteStartElement("step");
            Writer.WriteAttributeString("name", result.StringStep.ToString().TrimEnd());
            Writer.WriteAttributeString("outcome", result.Result.ToString());
            if (result.Result.GetType() == typeof(Failed))
                Writer.WriteElementString("failure", result.Message);
            if (result.StringStep is StringTableStep)
                DoStringTableStep(result.StringStep as StringTableStep);
            Writer.WriteEndElement();
        }

        private void DoStringTableStep(StringTableStep stringTableStep)
        {
            if (!stringTableStep.TableSteps.Any())
                return;
            Writer.WriteStartElement("table");
            WriteColumnNames(stringTableStep.TableSteps.First().ColumnNames);
            foreach (var tableStep in stringTableStep.TableSteps)
            {
                Writer.WriteStartElement("row");
                WriteExampleRow(tableStep);
                Writer.WriteEndElement();
            }
            Writer.WriteEndElement();
        }

        private void DoRunFinished()
        {
            Writer.WriteEndElement(); // </results>
            Writer.Flush();
        }

        private void DoExamplesInScenario(ScenarioExampleResult scenarioExampleResult)
        {
            if (scenarioExampleResult == null)
            {
                return;
            }

            Writer.WriteStartElement("examples");
            WriteColumnNames(scenarioExampleResult.Examples.First().ColumnNames);

            var scenarioResults = scenarioExampleResult.ExampleResults.ToArray();
            var idx = 0;
            foreach (var example in scenarioExampleResult.Examples)
            {
                Writer.WriteStartElement("example");
                Writer.WriteStartAttribute("outcome");
                Writer.WriteString(scenarioResults[idx++].Result.ToString());
                Writer.WriteEndAttribute();
                WriteExampleRow(example);
                Writer.WriteEndElement();
            }

            Writer.WriteEndElement();
        }

        private void WriteExampleRow(Example example)
        {
            foreach (var columnName in example.ColumnNames)
            {
                Writer.WriteStartElement("column");
                Writer.WriteStartAttribute("columnName");
                Writer.WriteString(columnName.Name);
                Writer.WriteEndAttribute();
                Writer.WriteString(example.ColumnValues[columnName.Name]);
                Writer.WriteEndElement();
            }
        }

        private void WriteColumnNames(ExampleColumns columnNames)
        {
            Writer.WriteStartElement("columnNames");

            foreach (var columnName in columnNames)
            {
                Writer.WriteStartElement("columnName");
                Writer.WriteString(columnName.Name);
                Writer.WriteEndElement();
            }

            Writer.WriteEndElement();
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
            var eventsToUse = events.Where(e => e.EventType == EventType.ScenarioStart);
            var sr = from s in storyResults
                     where HasScenario(eventsToUse, s.ScenarioResult.ScenarioTitle)
                           && scenarioResult(s.ScenarioResult.Result)
                     select s.ScenarioResult;
            return sr;
        }

        private bool IsPendingAndNoActionStepsResults(ScenarioResult scenarioResult)
        {
            return scenarioResult.Result.GetType() == typeof(Pending) && (scenarioResult.StepResults.Count() == 0);
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
                    Writer.WriteString(row + Environment.NewLine);

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
            Writer.WriteStartElement(elementName);
            Writer.WriteAttributeString("name", attributeName);
            Writer.WriteAttributeString("time", Math.Round(timeTaken.TotalSeconds, 1).ToString());
        }

        private void CreatePendingSteps(EventReceived evt, ScenarioResult scenarioResult)
        {
            var actionSteps = from e in EventsOf(evt, EventType.ScenarioResult)
                              where e.EventType == EventType.ScenarioStart
                              select e;
            foreach (var step in actionSteps)
                scenarioResult.AddActionStepResult(new StepResult(new StringStep(step.Message, "lost it"), new Pending(scenarioResult.Message)));
        }

        private int CountFeatures()
        {
            return CountEventsOfType(EventType.FeatureStart);
        }

        private int CountScenarios()
        {
            var storyResults = GetScenarioResults(EventsReceived, p => true);
            return storyResults.Count();
        }

        private int CountFailingScenarios()
        {
            return CountFailingScenarios(EventsReceived);
        }

        private int CountFailingScenarios(IEnumerable<EventReceived> events)
        {
            var scenarioResults = GetScenarioResults(events, s => s.GetType() == typeof(Failed));
            return scenarioResults.Count();
        }

        private int CountPendingScenarios()
        {
            return CountPendingScenarios(EventsReceived);
        }

        private int CountPendingScenarios(IEnumerable<EventReceived> events)
        {
            var scenarioResults = GetScenarioResults(events, s => s.GetType() == typeof(Pending));

            return scenarioResults.Count();
        }

        private int CountEventsOfType(EventType eventType)
        {
            var themes = from e in EventsReceived
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
            var idxStart = EventsReceived.IndexOf(startEvent);
            var idxEnd = idxStart;
            var events = new List<EventReceived>();
            do
            {
                events.Add(EventsReceived[idxEnd]);
                idxEnd++;
            }
            while (idxEnd < EventsReceived.Count && EventsReceived[idxEnd].EventType != endWithEvent);
            if (idxEnd < EventsReceived.Count)
            {
                events.Add(EventsReceived[idxEnd]);
            }

            return events;
        }
    }
}