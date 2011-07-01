using System;
using System.Xml;
using JetBrains.ReSharper.TaskRunnerFramework;

namespace NBehave.ReSharper.Plugin.UnitTestRunner
{
    [Serializable]
    public class NBehaveScenarioTask : RemoteTask, IEquatable<NBehaveScenarioTask>
    {
        public NBehaveScenarioTask(XmlElement element)
            : base(element)
        {
            FeatureFile = GetXmlAttribute(element, "featureFile");
            Scenario = GetXmlAttribute(element, "scenario");
        }

        public NBehaveScenarioTask(string featureFile, string scenario)
            : base(NBehaveTaskRunner.RunnerId)
        {
            FeatureFile = featureFile ?? "";
            Scenario = scenario;
        }

        public string FeatureFile { get; private set; }
        public string Scenario { get; private set; }

        public override bool IsMeaningfulTask
        {
            get { return true; }
        }

        public override void SaveXml(XmlElement element)
        {
            base.SaveXml(element);
            SetXmlAttribute(element, "featureFile", FeatureFile);
            SetXmlAttribute(element, "scenario", Scenario);
        }

        public override bool Equals(object obj)
        {
            return this == obj || Equals(obj as NBehaveScenarioTask);
        }

        public override bool Equals(RemoteTask other)
        {
            return Equals(other as NBehaveScenarioTask);
        }

        public bool Equals(NBehaveScenarioTask task)
        {
            return task != null
                   && FeatureFile == task.FeatureFile
                   && Scenario == task.Scenario;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = base.GetHashCode();
                result = (result * 397) ^ FeatureFile.GetHashCode();
                result = (result * 397) ^ Scenario.GetHashCode();
                return result;
            }
        }
    }
}