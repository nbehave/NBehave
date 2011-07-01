//using System.Collections.Generic;
//using JetBrains.ProjectModel;
//using JetBrains.ReSharper.Psi;
//using JetBrains.ReSharper.UnitTestFramework;
//using NBehave.ReSharper.Plugin.UnitTestRunner;

//namespace NBehave.ReSharper.Plugin.UnitTestProvider
//{
//    public class NBehaveFeatureFileTestElement : NBehaveUnitTestElementBase
//    {
//        public NBehaveFeatureFileTestElement(IProjectFile featureFile, TestProvider testProvider, ProjectModelElementEnvoy projectModel)
//            : base(featureFile, testProvider, featureFile.Location.FullPath, projectModel, null)
//        {
//        }

//        public override string ShortName
//        {
//            get { return FeatureFile; }
//        }

//        public override string Kind
//        {
//            get { return "NBehave feature file"; }
//        }

//        public override string GetPresentation()
//        {
//            return ShortName;
//        }

//        public override IList<UnitTestTask> GetTaskSequence(IEnumerable<IUnitTestElement> explicitElements)
//        {
//            var list = new List<UnitTestTask>
//                           {
//                               new UnitTestTask(null, new AssemblyTask(AssemblyOutFile)),
//                               //new UnitTestTask(this, new FeatureTask(FeatureFile))
//                           };
//            return list;
//        }

//        public override UnitTestElementDisposition GetDisposition()
//        {
//            //Denna metod anropas om man tex trycker på enter på en nod.
//            return null;
//        }

//        public override IDeclaredElement GetDeclaredElement()
//        {
//            return null;
//        }

//        public bool Equals(NBehaveFeatureFileTestElement other)
//        {
//            if (other == null)
//                return false;
//            if (ReferenceEquals(this, other)) return true;
//            return base.Equals(other) && Equals(other.FeatureFile, FeatureFile);
//        }

//        public override bool Equals(IUnitTestElement other)
//        {
//            return Equals(other as NBehaveFeatureFileTestElement);
//        }

//        public override bool Equals(object obj)
//        {
//            return Equals(obj as NBehaveFeatureFileTestElement);
//        }

//        public override int GetHashCode()
//        {
//            unchecked
//            {
//                return (base.GetHashCode() * 397) ^ FeatureFile.GetHashCode();
//            }
//        }
//    }
//}