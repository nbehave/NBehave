using System.Collections.Generic;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.UnitTestFramework;
using NBehave.Domain;

using NBehave.ReSharper.Plugin.UnitTestRunner;

namespace NBehave.ReSharper.Plugin.UnitTestProvider
{
    public class NBehaveExampleTestElement : NBehaveUnitTestElementBase
    {
        private readonly Example _example;
        private readonly string _identity;

        public NBehaveExampleTestElement(Example example, IProjectFile featureFile, IUnitTestProvider testProvider, ProjectModelElementEnvoy projectModel, NBehaveUnitTestElementBase parent)
            : base(featureFile, testProvider, parent.Id + "/" + example, projectModel, parent)
        {
            _example = example;
            _identity = IdentityGenerator.NextValue().ToString().PadLeft(9, '0');
        }

        public override string ShortName
        {
            get { return _identity + ": " + Example; }
        }

        public override string Kind
        {
            get { return "NBehave example"; }
        }

        public Example Example
        {
            get { return _example; }
        }

        public override string GetPresentation()
        {
            return Example.ColumnValuesToString();
        }

        public override IList<UnitTestTask> GetTaskSequence(IList<IUnitTestElement> explicitElements)
        {
            var parent = Parent as NBehaveExampleParentTestElement;
            string scenario = (parent.Parent is NBehaveScenarioTestElement) ? ((NBehaveScenarioTestElement)parent.Parent).Scenario : "";
            var taskSequence = (Parent != null) ? DoGetTaskSequence(explicitElements) : new List<UnitTestTask>();
            taskSequence.Add(new UnitTestTask(this, new NBehaveExampleTask(FeatureFile, scenario, _example)));
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

        public bool Equals(NBehaveExampleTestElement other)
        {
            if (other == null)
                return false;
            if (ReferenceEquals(this, other)) return true;
            return base.Equals(other) && Equals(other._example.ToString(), _example.ToString());
        }

        public override bool Equals(IUnitTestElement other)
        {
            return Equals(other as NBehaveExampleTestElement);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as NBehaveExampleTestElement);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (base.GetHashCode() * 397) ^ _example.GetHashCode();
            }
        }
    }
}