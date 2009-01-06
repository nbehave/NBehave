using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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


        public XmlOutputBase(XmlWriter writer, Queue<Action> actions)
        {
            Writer = writer;
            Actions = actions;
        }

        protected void WriteToStream(Timer result, string element, string name)
        {
            Writer.WriteStartElement(element);
            Writer.WriteAttributeString("name", name);
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
