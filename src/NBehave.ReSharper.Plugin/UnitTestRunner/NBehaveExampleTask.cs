using System;
using System.Xml;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.TaskRunnerFramework;

namespace NBehave.ReSharper.Plugin.UnitTestRunner
{
    [Serializable]
    public class NBehaveExampleTask : NBehaveRemoteTask, IEquatable<NBehaveExampleTask>
    {
        public NBehaveExampleTask(XmlElement element)
            : base(element)
        {
            Example = GetXmlAttribute(element, "example");
        }

        public NBehaveExampleTask(IProjectFile featureFile, string scenario, Example example)
            : base(featureFile, scenario)
        {
            Example = example.ToString();
        }

        public string Example { get; private set; }

        public override bool IsMeaningfulTask
        {
            get { return true; }
        }

        public override void SaveXml(XmlElement element)
        {
            base.SaveXml(element);
            SetXmlAttribute(element, "featureFile", FeatureFile);
            SetXmlAttribute(element, "scenario", Scenario);
            SetXmlAttribute(element, "example", Example);
        }

        public override bool Equals(object obj)
        {
            return this == obj || Equals(obj as NBehaveExampleTask);
        }

        public override bool Equals(RemoteTask other)
        {
            return Equals(other as NBehaveExampleTask);
        }

        public bool Equals(NBehaveExampleTask task)
        {
            return task != null
                   && FeatureFile == task.FeatureFile
                   && Scenario == task.Scenario
                   && Example.ToString() == task.Example.ToString();
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = FeatureFile.GetHashCode();
                result = (result * 397) ^ Scenario.GetHashCode();
                result = (result * 397) ^ Example.ToString().GetHashCode();
                return result;
            }
        }
    }
}