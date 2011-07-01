using System;
using System.Reflection;
using System.Xml;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.TaskRunnerFramework;
using JetBrains.ReSharper.UnitTestFramework;
using JetBrains.ReSharper.UnitTestFramework.AttributeChecker;
using JetBrains.UI;
using NBehave.ReSharper.Plugin.UnitTestRunner;

namespace NBehave.ReSharper.Plugin.UnitTestProvider
{
    [UnitTestProvider]
    public class TestProvider : IUnitTestProvider
    {
        public const string NBehaveId = "NBehave";

        private readonly UnitTestElementComparer _unitTestElementComparer = new UnitTestElementComparer(new[]
                                                                                            {
	                                                                                            //typeof(NBehaveFeatureFileTestElement) ,
	                                                                                            typeof(NBehaveFeatureTestElement) ,
	                                                                                            typeof(NBehaveScenarioTestElement) ,
	                                                                                            typeof(NBehaveStepTestElement)
                                                                                            });

        private readonly ISolution _soultion;
        private readonly UnitTestAttributeCache _unitTestAttributeCache;

        public TestProvider(ISolution solution, UnitTestAttributeCache unitTestAttributeCache)
        {
            _soultion = solution;
            _unitTestAttributeCache = unitTestAttributeCache;
        }

        public System.Drawing.Image Icon
        {
            get { return ImageLoader.GetImage("nbehave", new Assembly[0]); }
        }

        public ISolution Solution
        {
            get { return _soultion; }
        }

        public string ID
        {
            get
            {
                return NBehaveId;
            }
        }

        public string Name
        {
            get { return NBehaveId; }
        }

        public RemoteTaskRunnerInfo GetTaskRunnerInfo()
        {
            return new RemoteTaskRunnerInfo(typeof(NBehaveTaskRunner));
        }

        public bool IsElementOfKind(IDeclaredElement declaredElement, UnitTestElementKind elementKind)
        {
            throw new ApplicationException("IsElementOfKind");
            switch (elementKind)
            {
                case UnitTestElementKind.Unknown:
                    {
                        return !IsUnitTestStuff(declaredElement);
                    }
                case UnitTestElementKind.Test:
                    {
                        return IsUnitTest(declaredElement);
                    }
                case UnitTestElementKind.TestContainer:
                    {
                        return IsUnitTestContainer(declaredElement);
                    }
                case UnitTestElementKind.TestStuff:
                    {
                        return IsUnitTestStuff(declaredElement);
                    }
                default:
                    {
                        throw new ArgumentOutOfRangeException("elementKind");
                    }
            }
        }

        public bool IsElementOfKind(IUnitTestElement element, UnitTestElementKind elementKind)
        {
            throw new ApplicationException("IsElementOfKind");
            switch (elementKind)
            {
                case UnitTestElementKind.Unknown:
                    {
                        return !(element is NBehaveUnitTestElementBase);
                    }
                case UnitTestElementKind.Test:
                    {
                        return element is NBehaveStepTestElement;
                    }
                case UnitTestElementKind.TestContainer:
                    {
                        return element is NBehaveScenarioTestElement;
                    }
                case UnitTestElementKind.TestStuff:
                    {
                        return element is NBehaveFeatureTestElement;
                    }
                default:
                    {
                        throw new ArgumentOutOfRangeException("elementKind");
                    }
            }
        }

        private bool IsUnitTest(IDeclaredElement element)
        {
            var typeMember = element as ITypeMember;
            return typeMember != null && MetadataExplorer.IsActionStepMethod(typeMember, _unitTestAttributeCache);
        }

        private bool IsUnitTestContainer(IDeclaredElement element)
        {
            var typeElement = element as ITypeElement;
            bool flag;
            return typeElement != null && MetadataExplorer.IsActionStepsClass(typeElement, out flag, _unitTestAttributeCache);
        }

        private bool IsUnitTestStuff(IDeclaredElement declaredElement)
        {
            return IsUnitTest(declaredElement)
                    || IsUnitTestContainer(declaredElement);
        }

        public bool IsSupported(IHostProvider hostProvider)
        {
            return true;
        }

        public void ExploreSolution(ISolution solution, UnitTestElementConsumer consumer)
        {
        }

        public void ExploreExternal(UnitTestElementConsumer consumer)
        {
        }

        public int CompareUnitTestElements(IUnitTestElement x, IUnitTestElement y)
        {
            return _unitTestElementComparer.Compare(x, y);
        }

        public void SerializeElement(XmlElement parent, IUnitTestElement element)
        { }

        public IUnitTestElement DeserializeElement(XmlElement parent, IUnitTestElement parentElement)
        {
            return null;
        }
    }
}
