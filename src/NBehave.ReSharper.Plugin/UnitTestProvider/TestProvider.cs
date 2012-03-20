using System;
using System.Reflection;
using System.Xml;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.TaskRunnerFramework;
using JetBrains.ReSharper.UnitTestFramework;
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
	                                                                                            typeof(NBehaveFeatureTestElement) ,
	                                                                                            typeof(NBehaveBackgroundTestElement) ,
	                                                                                            typeof(NBehaveScenarioTestElement) ,
	                                                                                            typeof(NBehaveStepTestElement),
	                                                                                            typeof(NBehaveExampleTestElement),
	                                                                                            typeof(NBehaveExampleParentTestElement)
                                                                                            });

 #if RESHARPER_61
    public TestProvider()
    {
#else 
        public TestProvider(ISolution solution)
        {
            Solution = solution;
#endif
        }

        public System.Drawing.Image Icon
        {
            get { return ImageLoader.GetImage("nbehave", GetType().Assembly); }
        }

#if !RESHARPER_61
        public ISolution Solution { get; private set; }

        public void SerializeElement(XmlElement parent, IUnitTestElement element)
        { }

        public IUnitTestElement DeserializeElement(XmlElement parent, IUnitTestElement parentElement)
        {
            return null;
        }
#endif

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
            throw new NotImplementedException("It seems this method is used after all, please tell the NBehave team what you did to trigger this exception");
        }

        public bool IsElementOfKind(IUnitTestElement element, UnitTestElementKind elementKind)
        {
            throw new NotImplementedException("It seems this method is used after all, please tell the NBehave team what you did to trigger this exception");
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

    }
}
