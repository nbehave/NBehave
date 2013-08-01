using System;
using System.Xml;
using JetBrains.ProjectModel;
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

        public NBehaveFeatureTask(string featureTitle, IProjectFile featureFile)
            : base(NBehaveTaskRunner.RunnerId)
        {
            FeatureFile = featureFile.Location.FullPath;
            FeatureTitle = featureTitle;
        }

        public string FeatureTitle { get; private set; }
        public string FeatureFile { get; private set; }

        public override bool IsMeaningfulTask
        {
            get { return true; }
        }

        public override void SaveXml(XmlElement element)
        {
            base.SaveXml(element);
            SetXmlAttribute(element, "featureFile", FeatureFile);
            SetXmlAttribute(element, "featureTitle", FeatureTitle);
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
                   && FeatureTitle == task.FeatureTitle
                   && FeatureFile == task.FeatureFile;
        }

        public override int GetHashCode()
        {
            return FeatureFile.GetHashCode();
        }
    }
}