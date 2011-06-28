using System.Collections.Generic;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.UnitTestFramework;

namespace NBehave.ReSharper.Plugin
{
    public abstract class NBehaveUnitTestElementBase : IUnitTestElement
    {
        private readonly TestProvider _testProvider;
        private readonly string _id;
        private readonly ClrTypeName _typeName;
        private readonly ProjectModelElementEnvoy _projectModel;
        private readonly IList<IUnitTestElement> _children = new List<IUnitTestElement>();
        private NBehaveUnitTestElementBase _parent;
        private readonly IEnumerable<UnitTestElementCategory> _categories = new List<UnitTestElementCategory>(UnitTestElementCategory.Uncategorized);

        protected NBehaveUnitTestElementBase(TestProvider testProvider, string id, ClrTypeName typeName, ProjectModelElementEnvoy pointer, NBehaveUnitTestElementBase parent)
        {
            _testProvider = testProvider;
            _id = id;
            _typeName = typeName;
            _projectModel = pointer;
            Parent = parent;
        }

        public string Id
        {
            get { return _id; }
        }

        public ClrTypeName TypeName { get { return _typeName; } }
        
        public abstract string ShortName { get; }

        public UnitTestElementState State { get; set; }

        public IUnitTestProvider Provider
        {
            get { return _testProvider; }
        }

        public IUnitTestElement Parent
        {
            get { return _parent; }
            set
            {
                if (value == _parent)
                    return;
                if (_parent != null)
                {
                    _parent.RemoveChild(this);
                }

                _parent = (NBehaveUnitTestElementBase)value;
                if (_parent != null)
                    _parent.AppendChild(this);
            }
        }

        public ICollection<IUnitTestElement> Children
        {
            get { return _children; }
        }

        public string ExplicitReason { get; set; }

        public bool Explicit
        {
            get { return ExplicitReason != null; }
        }

        public abstract string Kind { get; }

        public IEnumerable<UnitTestElementCategory> Categories
        {
            get { return _categories; }
        }

        public abstract IList<UnitTestTask> GetTaskSequence(IEnumerable<IUnitTestElement> explicitElements);

        public IProject GetProject()
        {
            return _projectModel.GetValidProjectElement() as IProject;
        }

        public abstract string GetPresentation();

        public UnitTestNamespace GetNamespace()
        {
            return new UnitTestNamespace(_typeName.GetNamespaceName());
        }

        public abstract UnitTestElementDisposition GetDisposition();

        public abstract IDeclaredElement GetDeclaredElement();

        public abstract IEnumerable<IProjectFile> GetProjectFiles();

        private void AppendChild(IUnitTestElement element)
        {
            _children.Add(element);
        }

        private void RemoveChild(IUnitTestElement element)
        {
            _children.Add(element);
        }

        public abstract bool Equals(IUnitTestElement other);

        public override string ToString()
        {
            return Id;
        }
    }
}