using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.TaskRunnerFramework;
using NBehave.Narrator.Framework;

namespace NBehave.ReSharper.Plugin.UnitTestRunner
{
    [Serializable]
    public class NBehaveExampleParentTask : RemoteTask, IEquatable<NBehaveExampleParentTask>
    {
        public NBehaveExampleParentTask(XmlElement element)
            : base(element)
        {
            FeatureFile = GetXmlAttribute(element, "featureFile");
            Scenario = GetXmlAttribute(element, "scenario");
            int nrOfExamples = int.Parse(GetXmlAttribute(element, "examples"));
            var examples = new List<Example>();
            for (int i = 0; i < nrOfExamples; i++)
            {
                Example e = ExampleBuilder.BuildFromString(GetXmlAttribute(element, "example_" + i));
                examples.Add(e);
            }
            Examples = examples;
        }

        public NBehaveExampleParentTask(IProjectFile featureFile, string scenario, IEnumerable<Example> examples)
            : base(NBehaveTaskRunner.RunnerId)
        {
            FeatureFile = featureFile.Location.FullPath;
            Scenario = scenario;
            Examples = examples;
        }

        public string FeatureFile { get; private set; }
        public string Scenario { get; private set; }
        public IEnumerable<Example> Examples { get; private set; }

        public override bool IsMeaningfulTask
        {
            get { return true; }
        }

        public override void SaveXml(XmlElement element)
        {
            base.SaveXml(element);
            SetXmlAttribute(element, "featureFile", FeatureFile);
            SetXmlAttribute(element, "scenario", Scenario);
            SaveExamplesAsXml(element);
        }

        private void SaveExamplesAsXml(XmlElement element)
        {
            int nrOfExamples = Examples.Count();
            SetXmlAttribute(element, "examples", nrOfExamples.ToString());
            var examples = Examples.ToArray();
            for (int i = 0; i < Examples.Count(); i++)
                SetXmlAttribute(element, "example_" + i, examples[i].ToString());
        }

        public override bool Equals(object obj)
        {
            return this == obj || Equals(obj as NBehaveExampleParentTask);
        }

        public override bool Equals(RemoteTask other)
        {
            return Equals(other as NBehaveExampleParentTask);
        }

        public bool Equals(NBehaveExampleParentTask task)
        {
            return task != null
                   && FeatureFile == task.FeatureFile
                   && Scenario == task.Scenario
                   && Examples.AsString() == task.Examples.AsString();
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = FeatureFile.GetHashCode();
                result = (result * 397) ^ Scenario.GetHashCode();
                result = (result * 397) ^ Examples.AsString().GetHashCode();
                return result;
            }
        }
    }
}