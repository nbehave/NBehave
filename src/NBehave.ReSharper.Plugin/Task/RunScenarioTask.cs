using System;
using System.Xml;
using JetBrains.ReSharper.TaskRunnerFramework;

namespace NBehave.ReSharper.Plugin.Task
{
    [Serializable]
    public class RunScenarioTask : RemoteTask, IEquatable<RunScenarioTask>
    {
        private readonly string _featureFile;

        public RunScenarioTask(XmlElement element)
            : base(element)
        {
            _featureFile = GetXmlAttribute(element, "featureFile");
        }

        public RunScenarioTask(string featureFeatureFile)
            : base(NBehaveTaskRunner.RunnerId)
        {
            _featureFile = featureFeatureFile;
        }

        public string FeatureFile { get { return _featureFile; } }

        public override bool IsMeaningfulTask
        {
            get { return true; }
        }

        public override void SaveXml(XmlElement element)
        {
            base.SaveXml(element);
            SetXmlAttribute(element, "featureFile", _featureFile);
        }

        public override bool Equals(RemoteTask other)
        {
            return Equals(other as RunScenarioTask);
        }

        public override bool Equals(object obj)
        {
            return this == obj || Equals(obj as RunScenarioTask);
        }

        public bool Equals(RunScenarioTask task)
        {
            return task != null
                   && _featureFile == task._featureFile;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (base.GetHashCode() * 397) ^ (_featureFile.GetHashCode());
            }
        }
    }
}