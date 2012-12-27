using System.Collections.Generic;
using System.Linq;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.UnitTestFramework;

using NBehave.ReSharper.Plugin.UnitTestRunner;

namespace NBehave.ReSharper.Plugin.UnitTestProvider
{
    public class NBehaveExampleParentTestElement : NBehaveUnitTestElementBase
    {
        private readonly List<Example> _examples;
        private readonly string _identity;

        public NBehaveExampleParentTestElement(IEnumerable<Example> examples, IProjectFile featureFile, IUnitTestProvider testProvider, ProjectModelElementEnvoy projectModel, NBehaveUnitTestElementBase parent)
            : base(featureFile, testProvider, parent.Id + "/Examples", projectModel, parent)
        {
            _examples = new List<Example>(examples);
            _identity = IdentityGenerator.NextValue().ToString().PadLeft(9, '0');
        }

        public override string ShortName
        {
            get { return _identity + ": Examples"; }
        }

        public override string Kind
        {
            get { return "NBehave examples"; }
        }

        public override string GetPresentation()
        {
            var e = _examples.First();
            return e.ColumnNamesToString();
        }

        public override IList<UnitTestTask> GetTaskSequence(IList<IUnitTestElement> explicitElements)
        {
            var taskSequence = (Parent != null) ? DoGetTaskSequence(explicitElements) : new List<UnitTestTask>();
            string scenario = (Parent is NBehaveBackgroundTestElement) ? ((NBehaveBackgroundTestElement)Parent).Scenario : "";
            scenario = (Parent is NBehaveScenarioTestElement) ? ((NBehaveScenarioTestElement)Parent).Scenario : scenario;
            taskSequence.Add(new UnitTestTask(this, new NBehaveExampleParentTask(FeatureFile, scenario, _examples)));
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

        public bool Equals(NBehaveExampleParentTestElement other)
        {
            if (other == null)
                return false;
            if (ReferenceEquals(this, other)) return true;
            return base.Equals(other) && Equals(other._examples.AsString(), _examples.AsString());
        }

        public override bool Equals(IUnitTestElement other)
        {
            return Equals(other as NBehaveExampleParentTestElement);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as NBehaveExampleParentTestElement);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (base.GetHashCode() * 397) ^ _examples.AsString().GetHashCode();
            }
        }
    }
}