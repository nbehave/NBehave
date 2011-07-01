using System;
using System.Xml;
using JetBrains.ReSharper.TaskRunnerFramework;

namespace NBehave.ReSharper.Plugin.UnitTestRunner
{
    [Serializable]
    public class NBehaveFeatureTask : RemoteTask, IEquatable<NBehaveFeatureTask>
    {
        public NBehaveFeatureTask(XmlElement element)
            : base(element)
        {
            FeatureFile = GetXmlAttribute(element, "featureFile");
        }

        public NBehaveFeatureTask(string featureFeatureFile)
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
            return this == obj || Equals(obj as NBehaveFeatureTask);
        }

        public override bool Equals(RemoteTask other)
        {
            return Equals(other as NBehaveFeatureTask);
        }

        public bool Equals(NBehaveFeatureTask task)
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