using System;
using System.Xml;
using JetBrains.ReSharper.TaskRunnerFramework;

namespace NBehave.ReSharper.Plugin.UnitTestRunner
{
    [Serializable]
    public class NBehaveAssemblyTask : RemoteTask, IEquatable<NBehaveAssemblyTask>
    {
        public NBehaveAssemblyTask(XmlElement element)
            : base(element)
        {
            AssemblyFile = GetXmlAttribute(element, "assemblyFile");
        }

        public string AssemblyFile { get; private set; }

        public NBehaveAssemblyTask(string pathToAssembly)
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

        public bool Equals(NBehaveAssemblyTask other)
        {
            if (other == null)
                return false;
            if (ReferenceEquals(this, other)) return true;
            return other.AssemblyFile == AssemblyFile;
        }

        public override bool Equals(RemoteTask other)
        {
            return Equals(other as NBehaveAssemblyTask);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as NBehaveAssemblyTask);
        }

        public override int GetHashCode()
        {
            unchecked
            {
            return AssemblyFile.GetHashCode();
            }
        }
    }
}