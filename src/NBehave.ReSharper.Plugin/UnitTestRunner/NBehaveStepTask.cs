using System;
using System.Xml;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.TaskRunnerFramework;

namespace NBehave.ReSharper.Plugin.UnitTestRunner
{
    [Serializable]
    public class NBehaveRemoteTask : RemoteTask
    {

        public string FeatureFile { get; private set; }
        public string Scenario { get; private set; }

        public override bool IsMeaningfulTask
        {
            get { return true; }
        }

         public NBehaveRemoteTask(XmlElement element)
            : base(element)
        {
            FeatureFile = GetXmlAttribute(element, "featureFile");
            Scenario = GetXmlAttribute(element, "scenario");

        }

         public NBehaveRemoteTask(IProjectFile featureFile, string scenario)
            : base(NBehaveTaskRunner.RunnerId)
        {
            FeatureFile = featureFile.Location.FullPath;
            Scenario = scenario;
        }
    }

    [Serializable]
    public class NBehaveStepTask : NBehaveRemoteTask, IEquatable<NBehaveStepTask>
    {
        public string Step { get; private set; }

        public NBehaveStepTask(XmlElement element)
            : base(element)
        {
            Step = GetXmlAttribute(element, "step");

        }

        public NBehaveStepTask(IProjectFile featureFile, string scenario, string step)
            : base(featureFile, scenario)
        {
            Step = step;
        }

        public override void SaveXml(XmlElement element)
        {
            base.SaveXml(element);
            SetXmlAttribute(element, "featureFile", FeatureFile);
            SetXmlAttribute(element, "scenario", Scenario);
            SetXmlAttribute(element, "step", Step);
        }

        public override bool Equals(object obj)
        {
            return this == obj || Equals(obj as NBehaveStepTask);
        }

        public override bool Equals(RemoteTask other)
        {
            return Equals(other as NBehaveStepTask);
        }

        public bool Equals(NBehaveStepTask task)
        {
            return task != null
                   && FeatureFile == task.FeatureFile
                   && Scenario == task.Scenario
                   && Step == task.Step;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = base.GetHashCode();
                result = (result * 397) ^ FeatureFile.GetHashCode();
                result = (result * 397) ^ Scenario.GetHashCode();
                result = (result * 397) ^ Step.GetHashCode();
                return result;
            }
        }
    }
}