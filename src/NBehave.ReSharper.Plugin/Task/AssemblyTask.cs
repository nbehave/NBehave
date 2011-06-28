using System;
using System.Xml;
using JetBrains.ReSharper.TaskRunnerFramework;

namespace NBehave.ReSharper.Plugin.Task
{
    [Serializable]
    public class AssemblyTask : RemoteTask, IEquatable<AssemblyTask>
    {
        public AssemblyTask(XmlElement element)
            : base(element)
        {
            AssemblyFile = GetXmlAttribute(element, "assemblyFile");
        }

        public string AssemblyFile { get; private set; }

        public AssemblyTask(string pathToAssembly)
            : base(NBehaveTaskRunner.RunnerId)
        {
            AssemblyFile = pathToAssembly;
        }

        public override void SaveXml(XmlElement element)
        {
            base.SaveXml(element);
            SetXmlAttribute(element, "assemblyFile", AssemblyFile);
        }

        public override bool IsMeaningfulTask
        {
            get { return true; }
        }

        public bool Equals(AssemblyTask other)
        {
            if (other == null)
                return false;
            if (ReferenceEquals(this, other)) return true;
            return base.Equals(other) && Equals(other.AssemblyFile, AssemblyFile);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as AssemblyTask);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (base.GetHashCode() * 397) ^ AssemblyFile.GetHashCode();
            }
        }
    }
}