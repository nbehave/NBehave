using System;
using System.Collections.Generic;
using System.Xml;

namespace NBehave.Narrator.Framework.EventListeners.Xml
{
    public abstract class XmlOutputBase
    {
        protected XmlWriter Writer { get; private set; }
        protected Queue<Action> Actions { get; private set; }

        public int TotalScenarios { get; set; }
        public int TotalScenariosPending { get; set; }
        public int TotalScenariosFailed { get; set; }

        protected XmlOutputBase(XmlWriter writer, Queue<Action> actions)
        {
            Writer = writer;
            Actions = actions;
        }

        protected void WriteStartElement(string elementName, string attributeName, Timer result)
        {
            Writer.WriteStartElement(elementName);
            Writer.WriteAttributeString("name", attributeName);
            Writer.WriteAttributeString("time", result.TimeTaken.ToString());
        }

        protected void WriteScenarioResult()
        {
            Writer.WriteAttributeString("scenarios", TotalScenarios.ToString());
            Writer.WriteAttributeString("scenariosFailed", TotalScenariosFailed.ToString());
            Writer.WriteAttributeString("scenariosPending", TotalScenariosPending.ToString());
        }

        protected void UpdateSummary(XmlOutputBase output, StoryResults results)
        {
            output.TotalScenarios += results.NumberOfScenariosFound;
            output.TotalScenariosFailed += (results.NumberOfScenariosFound - results.NumberOfPassingScenarios - results.NumberOfPendingScenarios);
            output.TotalScenariosPending += results.NumberOfPendingScenarios;
        }

        public virtual void DoResults(StoryResults results)
        {
            UpdateSummary(this, results);
        }

    }
}
