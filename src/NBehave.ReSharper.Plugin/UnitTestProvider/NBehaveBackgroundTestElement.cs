using System.Collections.Generic;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.UnitTestFramework;
using NBehave.ReSharper.Plugin.UnitTestRunner;

namespace NBehave.ReSharper.Plugin.UnitTestProvider
{
    public class NBehaveBackgroundTestElement : NBehaveUnitTestElementBase
    {
        private readonly string _scenario;

        public NBehaveBackgroundTestElement(string scenario, IProjectFile featureFile, IUnitTestProvider testProvider, ProjectModelElementEnvoy projectModel, NBehaveUnitTestElementBase parent)
            : base(featureFile, testProvider, parent.Id + "/Background" , projectModel, parent)
        {
            _scenario = scenario;
        }

        public override string ShortName
        {
            get { return "Background to " + _scenario; }
        }

        public override string Kind
        {
            get { return "Background"; }
        }

        public override string GetPresentation()
        {
            return ShortName;
        }

        public override IList<UnitTestTask> GetTaskSequence(IEnumerable<IUnitTestElement> explicitElements)
        {
            var taskSequence = (Parent != null) ? Parent.GetTaskSequence(explicitElements) : new List<UnitTestTask>();
            taskSequence.Add(new UnitTestTask(this, new NBehaveBackgroundTask(FeatureFile, _scenario)));
            return taskSequence;
        }

        public override UnitTestElementDisposition GetDisposition()
        {
            //Denna metod anropas om man tex trycker på enter på en nod.
            return null;
        }

        public override IDeclaredElement GetDeclaredElement()
        {
            return null;
        }

        private bool Equals(NBehaveBackgroundTestElement other)
        {
            if (other == null)
                return false;
            if (ReferenceEquals(this, other)) return true;
            return base.Equals(other) && Equals(other._scenario, _scenario);
        }

        public override bool Equals(IUnitTestElement other)
        {
            return Equals(other as NBehaveBackgroundTestElement);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as NBehaveBackgroundTestElement);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (base.GetHashCode() * 397) ^ _scenario.GetHashCode();
            }
        }
    }
}