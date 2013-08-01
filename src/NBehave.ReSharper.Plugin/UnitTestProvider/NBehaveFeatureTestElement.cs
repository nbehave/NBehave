using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.UnitTestFramework;
using NBehave.ReSharper.Plugin.UnitTestRunner;

namespace NBehave.ReSharper.Plugin.UnitTestProvider
{
    public class NBehaveFeatureTestElement : NBehaveUnitTestElementBase
    {
        private readonly string _featureTitle;

        public NBehaveFeatureTestElement(string featureTitle, IProjectFile featureFile, IUnitTestProvider testProvider, ProjectModelElementEnvoy projectModel)
            : base(featureFile, testProvider, featureFile.Location.FullPath + "/" + featureTitle, projectModel, null)
        {
            _featureTitle = featureTitle;
            IdentityGenerator.Reset();
        }

        public string Feature { get { return _featureTitle; } }

        public override string ShortName
        {
            get
            {
                var rows = Feature.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
                string name = rows.FirstOrDefault() ?? _featureTitle;
                return name;
            }
        }

        public override string Kind
        {
            get { return "NBehave feature"; }
        }

        public override string GetPresentation(IUnitTestElement parent = null)
        {
            return Feature;
        }

        public override IList<UnitTestTask> GetTaskSequence(IList<IUnitTestElement> explicitElements)
        {
            var list = new List<UnitTestTask>
                                       {
                                           new UnitTestTask(null, new NBehaveAssemblyTask(AssemblyOutFile)),
                                           new UnitTestTask(this, new NBehaveFeatureTask(Feature, FeatureFile))
                                       };
            return list;
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

        public bool Equals(NBehaveFeatureTestElement other)
        {
            if (other == null)
                return false;
            if (ReferenceEquals(this, other)) return true;
            return base.Equals(other) && Equals(other._featureTitle, _featureTitle);
        }

        public override bool Equals(IUnitTestElement other)
        {
            return Equals(other as NBehaveFeatureTestElement);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as NBehaveFeatureTestElement);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (base.GetHashCode() * 397) ^ _featureTitle.GetHashCode();
            }
        }
    }
}