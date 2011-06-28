using System;
using System.Xml;
using JetBrains.ReSharper.TaskRunnerFramework;

namespace NBehave.ReSharper.Plugin.Task
{
    [Serializable]
    public class FeatureTask : RemoteTask, IEquatable<FeatureTask>
    {
        public FeatureTask(XmlElement element)
            : base(element)
        {
            FeatureFile = GetXmlAttribute(element, "featureFile");
        }

        public FeatureTask(string featureFeatureFile)
            : base(NBehaveTaskRunner.RunnerId)
        {
            FeatureFile = featureFeatureFile;
        }

        public string FeatureFile { get; private set; }

        public override bool IsMeaningfulTask
        {
            get { return true; }
        }

        public override void SaveXml(XmlElement element)
        {
            base.SaveXml(element);
            SetXmlAttribute(element, "featureFile", FeatureFile);
        }

        public override bool Equals(object obj)
        {
            return this == obj || Equals(obj as FeatureTask);
        }

        public override bool Equals(RemoteTask other)
        {
            return Equals(other as FeatureTask);
        }

        public bool Equals(FeatureTask task)
        {
            return task != null
                   && FeatureFile == task.FeatureFile;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (base.GetHashCode() * 397) ^ (FeatureFile.GetHashCode());
            }
        }
    }
}